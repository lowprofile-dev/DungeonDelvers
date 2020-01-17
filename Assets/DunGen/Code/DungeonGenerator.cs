using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DunGen.Graph;
using System.Collections;
using UnityEngine.Serialization;

using Random = System.Random;
using Debug = UnityEngine.Debug;


namespace DunGen
{
	public delegate void TileInjectionDelegate(Random randomStream, ref List<InjectedTile> tilesToInject);

	public enum AxisDirection
	{
		PosX,
		NegX,
		PosY,
		NegY,
		PosZ,
		NegZ,
	}

	[Serializable]
	public class DungeonGenerator : ISerializationCallbackReceiver
	{
		public const int CurrentFileVersion = 1;

		#region Legacy Properties

		// Legacy properties only exist to avoid breaking existing projects
		// Converting old data structures over to the new ones

		[SerializeField]
		[FormerlySerializedAs("AllowImmediateRepeats")]
		private bool allowImmediateRepeats = false;

		#endregion


		public int Seed;
		public bool ShouldRandomizeSeed = true;
		public Random RandomStream { get; protected set; }
		public int MaxAttemptCount = 20;
		public bool UseMaximumPairingAttempts = false;
		public int MaxPairingAttempts = 5;
		public bool IgnoreSpriteBounds = false;
		public AxisDirection UpDirection = AxisDirection.PosY;
		[FormerlySerializedAs("OverrideAllowImmediateRepeats")]
		public bool OverrideRepeatMode = false;
		public TileRepeatMode RepeatMode = TileRepeatMode.Allow;
		public bool OverrideAllowTileRotation = false;
		public bool AllowTileRotation = false;
		public bool DebugRender = false;
		public float LengthMultiplier = 1.0f;
		public bool PlaceTileTriggers = true;
		public int TileTriggerLayer = 2;
		public bool GenerateAsynchronously = false;
		public float MaxAsyncFrameMilliseconds = 50;
		public float PauseBetweenRooms = 0;
		public bool RestrictDungeonToBounds = false;
		public Bounds TilePlacementBounds = new Bounds();
		public float OverlapThreshold = 0.01f;
		public float Padding = 0f;
		public bool DisallowOverhangs = false;

		public Vector3 UpVector
		{
			get
			{
				switch (UpDirection)
				{
					case AxisDirection.PosX:
						return new Vector3(+1, 0, 0);
					case AxisDirection.NegX:
						return new Vector3(-1, 0, 0);
					case AxisDirection.PosY:
						return new Vector3(0, +1, 0);
					case AxisDirection.NegY:
						return new Vector3(0, -1, 0);
					case AxisDirection.PosZ:
						return new Vector3(0, 0, +1);
					case AxisDirection.NegZ:
						return new Vector3(0, 0, -1);

					default:
						throw new NotImplementedException("AxisDirection '" + UpDirection + "' not implemented");
				}
			}
		}

		public event GenerationStatusDelegate OnGenerationStatusChanged;
		public event TileInjectionDelegate TileInjectionMethods;
		public event Action Cleared;
		public event Action Retrying;

		public GameObject Root;
		public DungeonFlow DungeonFlow;
		public GenerationStatus Status { get; private set; }
		public GenerationStats GenerationStats { get; private set; }
		public int ChosenSeed { get; protected set; }
		public Dungeon CurrentDungeon { get { return currentDungeon; } }
		public bool IsGenerating { get; private set; }
		public bool IsAnalysis { get; set; }

		protected int retryCount;
		protected Dungeon currentDungeon;
		protected readonly Dictionary<TilePlacementResult, int> tilePlacementResultCounters = new Dictionary<TilePlacementResult, int>();
		protected readonly List<PreProcessTileData> preProcessData = new List<PreProcessTileData>();
		protected readonly List<GameObject> useableTiles = new List<GameObject>();
		protected int targetLength;
		protected List<InjectedTile> tilesPendingInjection;
		protected List<DungeonGeneratorPostProcessStep> postProcessSteps = new List<DungeonGeneratorPostProcessStep>();

		[SerializeField]
		private int fileVersion;
		private int nextNodeIndex;
		private DungeonArchetype currentArchetype;
		private GraphLine previousLineSegment;
		private List<Doorway> allDoorways = new List<Doorway>();
		private Dictionary<Tile, GameObject> placedTilePrefabs = new Dictionary<Tile, GameObject>();
		private Stopwatch yieldTimer = new Stopwatch();
		private Dictionary<Tile, InjectedTile> injectedTiles = new Dictionary<Tile, InjectedTile>();


		public DungeonGenerator()
		{
			GenerationStats = new GenerationStats();
		}

		public DungeonGenerator(GameObject root)
			: this()
		{
			Root = root;
		}

		public void Generate()
		{
			if (IsGenerating)
				return;

			IsAnalysis = false;
			IsGenerating = true;
			Wait(OuterGenerate());
		}

		public void Cancel()
		{
			if (!IsGenerating)
				return;

			Clear(true);
			IsGenerating = false;
		}

		public Dungeon DetachDungeon()
		{
			if (currentDungeon == null)
				return null;

			Dungeon dungeon = currentDungeon;
			currentDungeon = null;
			Root = null;
			Clear(true);

			return dungeon;
		}

		protected virtual IEnumerator OuterGenerate()
		{
			Clear(false);

			yieldTimer.Restart();

			Status = GenerationStatus.NotStarted;

# if UNITY_EDITOR
			// Validate the dungeon archetype if we're running in the editor
			DungeonArchetypeValidator validator = new DungeonArchetypeValidator(DungeonFlow);

			if (!validator.IsValid())
			{
				ChangeStatus(GenerationStatus.Failed);
				IsGenerating = false;
				yield break;
			}
#endif

			ChosenSeed = (ShouldRandomizeSeed) ? new Random().Next() : Seed;
			RandomStream = new Random(ChosenSeed);

			if (Root == null)
				Root = new GameObject(Constants.DefaultDungeonRootName);


			yield return Wait(InnerGenerate(false));

			IsGenerating = false;
		}

		private Coroutine Wait(IEnumerator routine)
		{
			if (GenerateAsynchronously)
				return CoroutineHelper.Start(routine);
			else
			{
				while (routine.MoveNext()) { }
				return null;
			}
		}

		public void RandomizeSeed()
		{
			Seed = new Random().Next();
		}

		protected virtual IEnumerator InnerGenerate(bool isRetry)
		{
			if (isRetry)
			{
				ChosenSeed = RandomStream.Next();
				RandomStream = new Random(ChosenSeed);


				if (retryCount >= MaxAttemptCount && Application.isEditor)
				{
					string errorText =	"Failed to generate the dungeon " + MaxAttemptCount + " times.\n" +
										"This could indicate a problem with the way the tiles are set up. Try to make sure most rooms have more than one doorway and that all doorways are easily accessible.\n" +
										"Here are a list of all reasons a tile placement had to be retried:";
					
					foreach (var pair in tilePlacementResultCounters)
						if (pair.Value > 0)
							errorText += "\n" + pair.Key + " (x" + pair.Value + ")";

					Debug.LogError(errorText);
					ChangeStatus(GenerationStatus.Failed);
					yield break;
				}

				retryCount++;
				GenerationStats.IncrementRetryCount();

				if (Retrying != null)
					Retrying();
			}
			else
			{
				retryCount = 0;
				GenerationStats.Clear();
			}

			currentDungeon = Root.GetComponent<Dungeon>();
			if (currentDungeon == null)
				currentDungeon = Root.AddComponent<Dungeon>();

			currentDungeon.DebugRender = DebugRender;
			currentDungeon.PreGenerateDungeon(this);

			Clear(false);
			targetLength = Mathf.RoundToInt(DungeonFlow.Length.GetRandom(RandomStream) * LengthMultiplier);
			targetLength = Mathf.Max(targetLength, 2);

			// Tile Injection
			GenerationStats.BeginTime(GenerationStatus.TileInjection);

			if (tilesPendingInjection == null)
				tilesPendingInjection = new List<InjectedTile>();
			else
				tilesPendingInjection.Clear();

			injectedTiles.Clear();
			GatherTilesToInject();

			// Pre-Processing
			GenerationStats.BeginTime(GenerationStatus.PreProcessing);
			PreProcess();

			// Main Path Generation
			GenerationStats.BeginTime(GenerationStatus.MainPath);
			yield return Wait(GenerateMainPath());

			// We may have had to retry when generating the main path, if so, the status will be either Complete or Failed and we should exit here
			if (Status == GenerationStatus.Complete || Status == GenerationStatus.Failed)
				yield break;

			// Branch Paths Generation
			GenerationStats.BeginTime(GenerationStatus.Branching);
			yield return Wait(GenerateBranchPaths());

			// If there are any required tiles missing from the tile injection stage, the generation process should fail
			foreach (var tileInjection in tilesPendingInjection)
				if (tileInjection.IsRequired)
				{
					yield return Wait(InnerGenerate(true));
					yield break;
				}

			// We may have missed some required injected tiles and have had to retry, if so, the status will be either Complete or Failed and we should exit here
			if (Status == GenerationStatus.Complete || Status == GenerationStatus.Failed)
				yield break;

			// Post-Processing
			yield return Wait(PostProcess());


			// Waiting one frame so objects are in their expected state
			yield return null;


			ChangeStatus(GenerationStatus.Complete);

			// Let DungenCharacters know that they should re-check the Tile they're in
			foreach (var character in Component.FindObjectsOfType<DungenCharacter>())
				character.ForceRecheckTile();
		}

		public virtual void Clear(bool stopCoroutines)
		{
			if(stopCoroutines)
				CoroutineHelper.StopAll();

			if (currentDungeon != null)
				currentDungeon.Clear();

			foreach (var p in preProcessData)
				UnityUtil.Destroy(p.Proxy);

			useableTiles.Clear();
			preProcessData.Clear();

			previousLineSegment = null;

			placedTilePrefabs.Clear();
			tilePlacementResultCounters.Clear();
			allDoorways.Clear();

			if (Cleared != null)
				Cleared();
		}

		private void ChangeStatus(GenerationStatus status)
		{
			var previousStatus = Status;
			Status = status;

			if (status == GenerationStatus.Failed)
				Clear(true);

			if (previousStatus != status && OnGenerationStatusChanged != null)
				OnGenerationStatusChanged(this, status);
		}

		protected virtual void PreProcess()
		{
			if (preProcessData.Count > 0)
				return;

			ChangeStatus(GenerationStatus.PreProcessing);

			var usedTileSets = DungeonFlow.GetUsedTileSets().Concat(tilesPendingInjection.Select(x => x.TileSet)).Distinct();

			foreach (var tileSet in usedTileSets)
				foreach (var tile in tileSet.TileWeights.Weights)
				{
					if (tile.Value != null)
					{
						useableTiles.Add(tile.Value);
						tile.TileSet = tileSet;
					}
				}
		}

		protected virtual void GatherTilesToInject()
		{
			Random injectionRandomStream = new Random(ChosenSeed);

			// Gather from DungeonFlow
			foreach(var rule in DungeonFlow.TileInjectionRules)
			{
				// Ignore invalid rules
				if (rule.TileSet == null || (!rule.CanAppearOnMainPath && !rule.CanAppearOnBranchPath))
					continue;

				bool isOnMainPath = (!rule.CanAppearOnBranchPath) ? true : (!rule.CanAppearOnMainPath) ? false : injectionRandomStream.NextDouble() > 0.5;

				InjectedTile injectedTile = new InjectedTile(	rule.TileSet,
																isOnMainPath,
																rule.NormalizedPathDepth.GetRandom(injectionRandomStream),
																rule.NormalizedBranchDepth.GetRandom(injectionRandomStream),
																rule.IsRequired);

				tilesPendingInjection.Add(injectedTile);
			}

			// Gather from external delegates
			if (TileInjectionMethods != null)
				TileInjectionMethods(injectionRandomStream, ref tilesPendingInjection);
		}

		protected virtual IEnumerator GenerateMainPath()
		{
			ChangeStatus(GenerationStatus.MainPath);
			nextNodeIndex = 0;
			var handledNodes = new List<GraphNode>(DungeonFlow.Nodes.Count);
			bool isDone = false;
			int i = 0;

			// Keep track of these now, we'll need them later when we know the actual length of the dungeon
			var tileSets = new List<List<TileSet>>(targetLength);
			var archetypes = new List<DungeonArchetype>(targetLength);
			var nodes = new List<GraphNode>(targetLength);
			var lines = new List<GraphLine>(targetLength);

			// We can't rigidly stick to the target length since we need at least one room for each node and that might be more than targetLength
			while (!isDone)
			{
				float depth = Mathf.Clamp(i / (float)(targetLength - 1), 0, 1);
				GraphLine lineSegment = DungeonFlow.GetLineAtDepth(depth);

				// This should never happen
				if (lineSegment == null)
				{
					yield return Wait(InnerGenerate(true));
					yield break;
				}

				// We're on a new line segment, change the current archetype
				if (lineSegment != previousLineSegment)
				{
					currentArchetype = lineSegment.DungeonArchetypes[RandomStream.Next(0, lineSegment.DungeonArchetypes.Count)];
					previousLineSegment = lineSegment;
				}

				List<TileSet> useableTileSets = null;
				GraphNode nextNode = null;
				var orderedNodes = DungeonFlow.Nodes.OrderBy(x => x.Position).ToArray();

				// Determine which node comes next
				foreach (var node in orderedNodes)
				{
					if (depth >= node.Position && !handledNodes.Contains(node))
					{
						nextNode = node;
						handledNodes.Add(node);
						break;
					}
				}

				// Assign the TileSets to use based on whether we're on a node or a line segment
				if (nextNode != null)
				{
					useableTileSets = nextNode.TileSets;
					nextNodeIndex = (nextNodeIndex >= orderedNodes.Length - 1) ? -1 : nextNodeIndex + 1;
					archetypes.Add(null);
					lines.Add(null);
					nodes.Add(nextNode);

					if (nextNode == orderedNodes[orderedNodes.Length - 1])
						isDone = true;
				}
				else
				{
					useableTileSets = currentArchetype.TileSets;
					archetypes.Add(currentArchetype);
					lines.Add(lineSegment);
					nodes.Add(null);
				}

				tileSets.Add(useableTileSets);

				i++;
			}

			int tileRetryCount = 0;
			int totalForLoopRetryCount = 0;

			for (int j = 0; j < tileSets.Count; j++)
			{
				var attachTo = (j == 0) ? null : currentDungeon.MainPathTiles[currentDungeon.MainPathTiles.Count - 1];
				var tile = AddTile(attachTo, tileSets[j], j / (float)(tileSets.Count - 1), archetypes[j]);

				// if no tile could be generated delete last successful tile and retry from previous index
				// else return false
				if (j > 5 && tile == null && tileRetryCount < 5 && totalForLoopRetryCount < 20)
				{
					Tile previousTile = currentDungeon.MainPathTiles[j - 1];

					foreach (var doorway in previousTile.Placement.AllDoorways)
						allDoorways.Remove(doorway);

					// If the tile we're removing was placed by tile injection, be sure to place the injected tile back on the pending list
					InjectedTile previousInjectedTile;
					if (injectedTiles.TryGetValue(previousTile, out previousInjectedTile))
					{
						tilesPendingInjection.Add(previousInjectedTile);
						injectedTiles.Remove(previousTile);
					}

					currentDungeon.RemoveLastConnection();
					currentDungeon.RemoveTile(previousTile);
					UnityUtil.Destroy(previousTile.gameObject);

					j -= 2; // -2 because loop adds 1
					tileRetryCount++;
					totalForLoopRetryCount++;
				}
				else if (tile == null)
				{
					yield return Wait(InnerGenerate(true));
					yield break;
				}
				else
				{
					tile.Node = nodes[j];
					tile.Line = lines[j];
					tileRetryCount = 0;


					// Wait for a frame to allow for animated loading screens, etc
					if (ShouldSkipFrame(true))
						yield return GetRoomPause();
				}
			}

			yield break; // Required for generation to run synchronously
		}

		private bool ShouldSkipFrame(bool isRoomPlacement)
		{
			if (!GenerateAsynchronously)
				return false;

			if (isRoomPlacement && PauseBetweenRooms > 0)
				return true;
			else
			{
				bool frameWasTooLong = yieldTimer.Elapsed.TotalMilliseconds >= MaxAsyncFrameMilliseconds;

				if (frameWasTooLong)
				{
					yieldTimer.Restart();
					return true;
				}
				else
					return false;
			}
		}

		private YieldInstruction GetRoomPause()
		{
			if (PauseBetweenRooms > 0)
				return new WaitForSeconds(PauseBetweenRooms);
			else
				return null;
		}

		protected virtual IEnumerator GenerateBranchPaths()
		{
			ChangeStatus(GenerationStatus.Branching);

			int[] mainPathBranches = new int[currentDungeon.MainPathTiles.Count];
			BranchCountHelper.ComputeBranchCounts(DungeonFlow, RandomStream, currentDungeon, ref mainPathBranches);

			for (int b = 0; b < mainPathBranches.Length; b++)
			{
				var tile = currentDungeon.MainPathTiles[b];
				int branchCount = mainPathBranches[b];

				// This tile was created from a graph node, there should be no branching
				if (tile.Archetype == null)
					continue;

				if (branchCount == 0)
					continue;

				for (int i = 0; i < branchCount; i++)
				{
					Tile previousTile = tile;
					int branchDepth = tile.Archetype.BranchingDepth.GetRandom(RandomStream);

					for (int j = 0; j < branchDepth; j++)
					{
						List<TileSet> useableTileSets;

						if (j == (branchDepth - 1) && tile.Archetype.GetHasValidBranchCapTiles())
						{
							if (tile.Archetype.BranchCapType == BranchCapType.InsteadOf)
								useableTileSets = tile.Archetype.BranchCapTileSets;
							else
								useableTileSets = tile.Archetype.TileSets.Concat(tile.Archetype.BranchCapTileSets).ToList();
						}
						else
							useableTileSets = tile.Archetype.TileSets;

						float normalizedDepth = (branchDepth <= 1) ? 1 : j / (float)(branchDepth - 1);
						var newTile = AddTile(previousTile, useableTileSets, normalizedDepth, tile.Archetype);

						if (newTile == null)
							break;

						newTile.Placement.BranchDepth = j;
						newTile.Placement.NormalizedBranchDepth = normalizedDepth;
						newTile.Node = previousTile.Node;
						newTile.Line = previousTile.Line;
						previousTile = newTile;

						// Wait for a frame to allow for animated loading screens, etc
						if (ShouldSkipFrame(true))
							yield return GetRoomPause();
					}
				}
			}

			yield break;
		}

		protected virtual Tile AddTile(Tile attachTo, IEnumerable<TileSet> useableTileSets, float normalizedDepth, DungeonArchetype archetype, TilePlacementResult result = TilePlacementResult.None)
		{
			bool isOnMainPath = (Status == GenerationStatus.MainPath);
			bool isFirstTile = attachTo == null;

			// Check list of tiles to inject
			InjectedTile chosenInjectedTile = null;
			int injectedTileIndexToRemove = -1;

			bool isPlacingSpecificRoom = isOnMainPath && (archetype == null);

			if (tilesPendingInjection != null && !isPlacingSpecificRoom)
			{
				float pathDepth = (isOnMainPath) ? normalizedDepth : attachTo.Placement.PathDepth / (float)(targetLength - 1);
				float branchDepth = (isOnMainPath) ? 0 : normalizedDepth;

				for (int i = 0; i < tilesPendingInjection.Count; i++)
				{
					var injectedTile = tilesPendingInjection[i];

					if (injectedTile.ShouldInjectTileAtPoint(isOnMainPath, pathDepth, branchDepth))
					{
						chosenInjectedTile = injectedTile;
						injectedTileIndexToRemove = i;

						break;
					}
				}
			}


			// Select appropriate tile weights
			IEnumerable<GameObjectChance> chanceEntries;

			if (chosenInjectedTile != null)
				chanceEntries = new List<GameObjectChance>(chosenInjectedTile.TileSet.TileWeights.Weights);
			else
				chanceEntries = useableTileSets.SelectMany(x => x.TileWeights.Weights);


			// Apply constraint overrides
			bool allowRotation = (isFirstTile) ? false : attachTo.AllowRotation;

			if (OverrideAllowTileRotation)
				allowRotation = AllowTileRotation;



			DoorwayPairFinder doorwayPairFinder = new DoorwayPairFinder()
			{
				RandomStream = RandomStream,
				Archetype = archetype,
				GetTileTemplateDelegate = GetTileTemplate,
				IsOnMainPath = isOnMainPath,
				NormalizedDepth = normalizedDepth,
				PreviousTile = attachTo,
				PreviousPrefab = (attachTo == null) ? null : placedTilePrefabs[attachTo],
				UpVector = UpVector,
				AllowRotation = allowRotation,
				TileWeights = new List<GameObjectChance>(chanceEntries),

				IsTileAllowedPredicate = (Tile previousTile, GameObject previousPrefab, Tile potentialNextTile, GameObject potentialNextPrefab, ref float weight) =>
				{
					bool isImmediateRepeat = (potentialNextPrefab == previousPrefab);
					var repeatMode = TileRepeatMode.Allow;

					if (OverrideRepeatMode)
						repeatMode = RepeatMode;
					else if (potentialNextTile != null)
						repeatMode = potentialNextTile.RepeatMode;

					bool allowTile = true;

					switch (repeatMode)
					{
						case TileRepeatMode.Allow:
							allowTile = true;
							break;

						case TileRepeatMode.DisallowImmediate:
							allowTile = !isImmediateRepeat;
							break;

						case TileRepeatMode.Disallow:
							allowTile = !placedTilePrefabs.Values.Contains(potentialNextPrefab);
							break;

						default:
							throw new NotImplementedException("TileRepeatMode " + repeatMode + " is not implemented");
					}

					return allowTile;
				},
			};

			int? maxPairingAttempts = (UseMaximumPairingAttempts) ? (int?)MaxPairingAttempts : null;
			Queue<DoorwayPair> pairsToTest = doorwayPairFinder.GetDoorwayPairs(maxPairingAttempts);
			TilePlacementResult lastTileResult = TilePlacementResult.NoValidTile;
			Tile createdTile = null;

			while(pairsToTest.Count > 0)
			{
				var pair = pairsToTest.Dequeue();

				lastTileResult = TryPlaceTile(pair, archetype, out createdTile);

				if (lastTileResult == TilePlacementResult.None)
					break;
				else
					AddTilePlacementResult(lastTileResult);
			}

			// Successfully placed the tile
			if (lastTileResult == TilePlacementResult.None)
			{
				// We've successfully injected the tile, so we can remove it from the pending list now
				if (chosenInjectedTile != null)
				{
					injectedTiles[createdTile] = chosenInjectedTile;
					tilesPendingInjection.RemoveAt(injectedTileIndexToRemove);

					if (isOnMainPath)
						targetLength++;
				}

				return createdTile;
			}
			else
				return null;
		}

		protected void AddTilePlacementResult(TilePlacementResult result)
		{
			int count;

			if (!tilePlacementResultCounters.TryGetValue(result, out count))
				tilePlacementResultCounters[result] = 1;
			else
				tilePlacementResultCounters[result] = count + 1;
		}

		protected TilePlacementResult TryPlaceTile(DoorwayPair pair, DungeonArchetype archetype, out Tile tile)
		{
			tile = null;

			var toTemplate = pair.NextTemplate;
			var fromDoorway = pair.PreviousDoorway;

			if (toTemplate == null)
				return TilePlacementResult.TemplateIsNull;

			int toDoorwayIndex = pair.NextTemplate.Doorways.IndexOf(pair.NextDoorway);

			if (fromDoorway != null)
			{
				// Move the proxy object into position
				GameObject toProxyDoor = toTemplate.ProxySockets[toDoorwayIndex];
				UnityUtil.PositionObjectBySocket(toTemplate.Proxy, toProxyDoor, fromDoorway.gameObject);

#if UNITY_2017_2_OR_NEWER
				// We need to manually sync collider transforms to avoid tiles having incorrect positions when we check for collisions
				if (!Physics.autoSyncTransforms)
					Physics.SyncTransforms();
#endif

				Bounds proxyBounds = toTemplate.Proxy.GetComponent<Collider>().bounds;

				// Check if the new tile is outside of the valid bounds
				if (RestrictDungeonToBounds && !TilePlacementBounds.Contains(proxyBounds))
					return TilePlacementResult.OutOfBounds;

				// Check if the new tile is colliding with any other
				if (IsCollidingWithAnyTile(proxyBounds, fromDoorway.Tile))
					return TilePlacementResult.TileIsColliding;
			}

			TilePlacementData newTile = new TilePlacementData(toTemplate, (Status == GenerationStatus.MainPath), archetype, pair.NextTileSet, currentDungeon);

			if (newTile == null)
				return TilePlacementResult.NewTileIsNull;

			if (newTile.IsOnMainPath)
			{
				if (pair.PreviousTile != null)
					newTile.PathDepth = pair.PreviousTile.Placement.PathDepth + 1;
			}
			else
			{
				newTile.PathDepth = pair.PreviousTile.Placement.PathDepth;
				newTile.BranchDepth = (pair.PreviousTile.Placement.IsOnMainPath) ? 0 : pair.PreviousTile.Placement.BranchDepth + 1;
			}

			if (fromDoorway != null)
			{
				newTile.Root.transform.parent = Root.transform;
				Doorway toDoorway = newTile.AllDoorways[toDoorwayIndex];

				UnityUtil.PositionObjectBySocket(newTile.Root, toDoorway.gameObject, fromDoorway.gameObject);
				currentDungeon.MakeConnection(fromDoorway, toDoorway, RandomStream);
			}
			else
			{
				newTile.Root.transform.parent = Root.transform;
				newTile.Root.transform.localPosition = Vector3.zero;
				newTile.Root.transform.localRotation = Quaternion.identity;
			}

#if UNITY_2017_2_OR_NEWER
			// We need to manually sync collider transforms to avoid tiles having incorrect positions when we check for collisions
			if (!Physics.autoSyncTransforms)
				Physics.SyncTransforms();
#endif

			currentDungeon.AddTile(newTile.Tile);

			if (!newTile.Tile.OverrideAutomaticTileBounds)
				newTile.RecalculateBounds(IgnoreSpriteBounds, UpVector);
			else
			{
				newTile.Bounds = newTile.Tile.transform.TransformBounds(newTile.Tile.TileBoundsOverride);
				newTile.LocalBounds = newTile.Tile.TileBoundsOverride;
			}

			if (PlaceTileTriggers)
			{
				newTile.Tile.AddTriggerVolume();
				newTile.Root.layer = TileTriggerLayer;
			}

			allDoorways.AddRange(newTile.AllDoorways);

			tile = newTile.Tile;
			placedTilePrefabs[tile] = toTemplate.Prefab;

			return TilePlacementResult.None;
		}

		protected PreProcessTileData GetTileTemplate(GameObject prefab)
		{
			var template = preProcessData.Where(x => { return x.Prefab == prefab; }).FirstOrDefault();

			// No proxy has been loaded yet, we should create one
			if (template == null)
			{
				template = new PreProcessTileData(prefab, IgnoreSpriteBounds, UpVector);
				preProcessData.Add(template);
			}

			return template;
		}

		protected PreProcessTileData PickRandomTemplate(DoorwaySocket socketGroupFilter)
		{
			// Pick a random tile
			var tile = useableTiles[RandomStream.Next(0, useableTiles.Count)];
			var template = GetTileTemplate(tile);

			// If there's a socket group filter and the chosen Tile doesn't have a socket of this type, try again
			if (socketGroupFilter != null && !template.DoorwaySockets.Contains(socketGroupFilter))
				return PickRandomTemplate(socketGroupFilter);

			return template;
		}

		protected int NormalizedDepthToIndex(float normalizedDepth)
		{
			return Mathf.RoundToInt(normalizedDepth * (targetLength - 1));
		}

		protected float IndexToNormalizedDepth(int index)
		{
			return index / (float)targetLength;
		}

		protected bool IsCollidingWithAnyTile(Bounds proxyBounds, Tile previousTile)
		{
			foreach (var t in currentDungeon.AllTiles)
			{
				bool isConnected = previousTile == t;
				float maxOverlap = (isConnected) ? OverlapThreshold : -Padding;

				if(DisallowOverhangs && !isConnected)
				{
					Vector3 overlaps = UnityUtil.CalculatePerAxisOverlap(t.Placement.Bounds, proxyBounds);
					float overlap = 0f;

					// Check for overlaps only along the ground plane, disregarding the up-axis
					// E.g. For +Y up, check for overlaps along X & Z axes
					switch (UpDirection)
					{
						case AxisDirection.PosX:
						case AxisDirection.NegX:
							overlap = Mathf.Min(overlaps.y, overlaps.z);
							break;

						case AxisDirection.PosY:
						case AxisDirection.NegY:
							overlap = Mathf.Min(overlaps.x, overlaps.z);
							break;

						case AxisDirection.PosZ:
						case AxisDirection.NegZ:
							overlap = Mathf.Min(overlaps.x, overlaps.y);
							break;

						default:
							throw new NotImplementedException("AxisDirection '" + UpDirection + "' is not implemented");
					}

					if(overlap > maxOverlap)
						return true;
				}
				else
				{
					float overlap = UnityUtil.CalculateOverlap(t.Placement.Bounds, proxyBounds);

					if(overlap > maxOverlap)
						return true;
				}
			}

			return false;
		}

		protected void ClearPreProcessData()
		{
			foreach (var p in preProcessData)
				UnityUtil.Destroy(p.Proxy);

			preProcessData.Clear();
		}

		protected virtual void ConnectOverlappingDoorways(float globalChance)
		{
			const float epsilon = 0.00001f;
			var processedDoorways = new List<Doorway>(allDoorways.Count);

			foreach (var a in allDoorways)
			{
				foreach (var b in allDoorways)
				{
					// Don't try to connect doorways to themselves
					if (a == b || a.Tile == b.Tile)
						continue;

					// We've already used this doorway
					if (processedDoorways.Contains(b))
						continue;

					// These doors cannot be connected due to their sockets
					if (!DoorwaySocket.CanSocketsConnect(a.Socket, b.Socket))
						continue;

					float distanceSqrd = (a.transform.position - b.transform.position).sqrMagnitude;

					// The doorways are too far apart
					if (distanceSqrd >= epsilon)
						continue;

					if(DungeonFlow.RestrictConnectionToSameSection)
					{
						bool tilesAreOnSameLineSegment = a.Tile.Line == b.Tile.Line;

						// The tiles are not on a line segment
						if (a.Tile.Line == null)
							tilesAreOnSameLineSegment = false;

						if (!tilesAreOnSameLineSegment)
							continue;
					}

					float chance = globalChance;

					// Allow tiles to override the global connection chance
					// If both tiles want to override the connection chance, use the lowest value
					if (a.Tile.OverrideConnectionChance && b.Tile.OverrideConnectionChance)
						chance = Mathf.Min(a.Tile.ConnectionChance, b.Tile.ConnectionChance);
					else if (a.Tile.OverrideConnectionChance)
						chance = a.Tile.ConnectionChance;
					else if (b.Tile.OverrideConnectionChance)
						chance = b.Tile.ConnectionChance;

					// There is no chance to connect these doorways
					if (chance <= 0f)
						continue;

					if (RandomStream.NextDouble() < chance)
						currentDungeon.MakeConnection(a, b, RandomStream);
				}

				processedDoorways.Add(a);
			}
		}

		/// <summary>
		/// Registers a post-process step with the generator which allows for a callback function to be invoked during the PostProcess step
		/// </summary>
		/// <param name="postProcessCallback">The callback to invoke</param>
		/// <param name="priority">The priority which determines the order in which post-process steps are invoked (highest to lowest).</param>
		/// <param name="phase">Which phase to run the post-process step. Used to determine whether the step should run before or after DunGen's built-in post-processing</param>
		public void RegisterPostProcessStep(Action<DungeonGenerator> postProcessCallback, int priority = 0, PostProcessPhase phase = PostProcessPhase.AfterBuiltIn)
		{
			postProcessSteps.Add(new DungeonGeneratorPostProcessStep(postProcessCallback, priority, phase));
		}

		/// <summary>
		/// Unregisters an existing post-process step registered using RegisterPostProcessStep()
		/// </summary>
		/// <param name="postProcessCallback">The callback to remove</param>
		public void UnregisterPostProcessStep(Action<DungeonGenerator> postProcessCallback)
		{
			for (int i = 0; i < postProcessSteps.Count; i++)
				if (postProcessSteps[i].PostProcessCallback == postProcessCallback)
					postProcessSteps.RemoveAt(i);
		}

		protected virtual IEnumerator PostProcess()
		{
			// Waiting one frame so objects are in their expected state
			yield return null;


			GenerationStats.BeginTime(GenerationStatus.PostProcessing);
			ChangeStatus(GenerationStatus.PostProcessing);

			// Order post-process steps by priority
			postProcessSteps.Sort((a, b) =>
			{
				return b.Priority.CompareTo(a.Priority);
			});

			// Apply any post-process to be run BEFORE built-in post-processing is run
			foreach (var step in postProcessSteps)
			{
				if (ShouldSkipFrame(false))
					yield return null;

				if (step.Phase == PostProcessPhase.BeforeBuiltIn)
					step.PostProcessCallback(this);
			}


			// Waiting one frame so objects are in their expected state
			yield return null;

			foreach (var tile in currentDungeon.AllTiles)
				tile.gameObject.SetActive(true);

			int length = currentDungeon.MainPathTiles.Count;

			//
			// Need to sort list manually to avoid compilation problems on iOS
			int maxBranchDepth = 0;

			if (currentDungeon.BranchPathTiles.Count > 0)
			{
				List<Tile> branchTiles = currentDungeon.BranchPathTiles.ToList();
				branchTiles.Sort((a, b) =>
				{
					return b.Placement.BranchDepth.CompareTo(a.Placement.BranchDepth);
				}
				);

				maxBranchDepth = branchTiles[0].Placement.BranchDepth;
			}
			// End calculate max branch depth
			//

			if (!IsAnalysis)
			{
				ConnectOverlappingDoorways(DungeonFlow.DoorwayConnectionChance);

				foreach (var tile in currentDungeon.AllTiles)
				{
					if (ShouldSkipFrame(false))
						yield return null;

					tile.Placement.NormalizedPathDepth = tile.Placement.PathDepth / (float)(length - 1);
					tile.Placement.ProcessDoorways(RandomStream);
				}

				currentDungeon.PostGenerateDungeon(this);

				// Process random props
				foreach (var tile in currentDungeon.AllTiles)
					foreach (var prop in tile.GetComponentsInChildren<RandomProp>())
					{
						if (ShouldSkipFrame(false))
							yield return null;

						prop.Process(RandomStream, tile);
					}

				ProcessGlobalProps();

				if (DungeonFlow.KeyManager != null)
					PlaceLocksAndKeys();
			}

			GenerationStats.SetRoomStatistics(currentDungeon.MainPathTiles.Count, currentDungeon.BranchPathTiles.Count, maxBranchDepth);
			ClearPreProcessData();


			// Waiting one frame so objects are in their expected state
			yield return null;


			// Apply any post-process to be run AFTER built-in post-processing is run
			foreach (var step in postProcessSteps)
			{
				if (ShouldSkipFrame(false))
					yield return null;

				if (step.Phase == PostProcessPhase.AfterBuiltIn)
					step.PostProcessCallback(this);
			}


			// Finalise
			GenerationStats.EndTime();

			// Activate all door gameobjects that were added to doorways
			foreach (var door in currentDungeon.Doors)
				if (door != null)
					door.SetActive(true);
		}

		protected virtual void ProcessGlobalProps()
		{
			Dictionary<int, GameObjectChanceTable> globalPropWeights = new Dictionary<int, GameObjectChanceTable>();

			foreach (var tile in currentDungeon.AllTiles)
			{
				foreach (var prop in tile.GetComponentsInChildren<GlobalProp>())
				{
					GameObjectChanceTable table = null;

					if (!globalPropWeights.TryGetValue(prop.PropGroupID, out table))
					{
						table = new GameObjectChanceTable();
						globalPropWeights[prop.PropGroupID] = table;
					}

					float weight = (tile.Placement.IsOnMainPath) ? prop.MainPathWeight : prop.BranchPathWeight;
					weight *= prop.DepthWeightScale.Evaluate(tile.Placement.NormalizedDepth);

					table.Weights.Add(new GameObjectChance(prop.gameObject, weight, 0, null));
				}
			}

			foreach (var chanceTable in globalPropWeights.Values)
				foreach (var weight in chanceTable.Weights)
					weight.Value.SetActive(false);

			List<int> processedPropGroups = new List<int>(globalPropWeights.Count);

			foreach (var pair in globalPropWeights)
			{
				if (processedPropGroups.Contains(pair.Key))
				{
					Debug.LogWarning("Dungeon Flow contains multiple entries for the global prop group ID: " + pair.Key + ". Only the first entry will be used.");
					continue;
				}

				var prop = DungeonFlow.GlobalProps.Where(x => x.ID == pair.Key).FirstOrDefault();

				if (prop == null)
					continue;

				var weights = pair.Value.Clone();
				int propCount = prop.Count.GetRandom(RandomStream);
				propCount = Mathf.Clamp(propCount, 0, weights.Weights.Count);

				for (int i = 0; i < propCount; i++)
				{
					var chosenEntry = weights.GetRandom(RandomStream, true, 0, null, true, true);

					if (chosenEntry != null && chosenEntry.Value != null)
						chosenEntry.Value.SetActive(true);
				}

				processedPropGroups.Add(pair.Key);
			}
		}

		protected virtual void PlaceLocksAndKeys()
		{
			var nodes = currentDungeon.ConnectionGraph.Nodes.Select(x => x.Tile.Node).Where(x => { return x != null; }).Distinct().ToArray();
			var lines = currentDungeon.ConnectionGraph.Nodes.Select(x => x.Tile.Line).Where(x => { return x != null; }).Distinct().ToArray();

			Dictionary<Doorway, Key> lockedDoorways = new Dictionary<Doorway, Key>();

			// Lock doorways on nodes
			foreach (var node in nodes)
			{
				foreach (var l in node.Locks)
				{
					var tile = currentDungeon.AllTiles.Where(x => { return x.Node == node; }).FirstOrDefault();
					var connections = currentDungeon.ConnectionGraph.Nodes.Where(x => { return x.Tile == tile; }).FirstOrDefault().Connections;
					Doorway entrance = null;
					Doorway exit = null;

					foreach (var conn in connections)
					{
						if (conn.DoorwayA.Tile == tile)
							exit = conn.DoorwayA;
						else if (conn.DoorwayB.Tile == tile)
							entrance = conn.DoorwayB;
					}

					var key = node.Graph.KeyManager.GetKeyByID(l.ID);

					if (entrance != null && (node.LockPlacement & NodeLockPlacement.Entrance) == NodeLockPlacement.Entrance)
						lockedDoorways.Add(entrance, key);

					if (exit != null && (node.LockPlacement & NodeLockPlacement.Exit) == NodeLockPlacement.Exit)
						lockedDoorways.Add(exit, key);
				}
			}

			// Lock doorways on lines
			foreach (var line in lines)
			{
				var doorways = currentDungeon.ConnectionGraph.Connections.Where(x =>
				{
					bool isDoorwayAlreadyLocked = lockedDoorways.ContainsKey(x.DoorwayA) || lockedDoorways.ContainsKey(x.DoorwayB);
					bool doorwayHasLockPrefabs = x.DoorwayA.Tile.TileSet.LockPrefabs.Count > 0;

					return x.DoorwayA.Tile.Line == line &&
							x.DoorwayB.Tile.Line == line &&
							!isDoorwayAlreadyLocked &&
							doorwayHasLockPrefabs;

				}).Select(x => x.DoorwayA).ToList();

				if (doorways.Count == 0)
					continue;

				foreach (var l in line.Locks)
				{
					int lockCount = l.Range.GetRandom(RandomStream);
					lockCount = Mathf.Clamp(lockCount, 0, doorways.Count);

					for (int i = 0; i < lockCount; i++)
					{
						if (doorways.Count == 0)
							break;

						var doorway = doorways[RandomStream.Next(0, doorways.Count)];
						doorways.Remove(doorway);

						if (lockedDoorways.ContainsKey(doorway))
							continue;

						var key = line.Graph.KeyManager.GetKeyByID(l.ID);
						lockedDoorways.Add(doorway, key);
					}
				}
			}

			List<Doorway> locksToRemove = new List<Doorway>();

			foreach (var pair in lockedDoorways)
			{
				var door = pair.Key;
				var key = pair.Value;
				List<Tile> possibleSpawnTiles = new List<Tile>();

				foreach (var t in currentDungeon.AllTiles)
				{
					if (t.Placement.NormalizedPathDepth >= door.Tile.Placement.NormalizedPathDepth)
						continue;

					bool canPlaceKey = false;

					if (t.Node != null && t.Node.Keys.Where(x => { return x.ID == key.ID; }).Count() > 0)
						canPlaceKey = true;
					else if (t.Line != null && t.Line.Keys.Where(x => { return x.ID == key.ID; }).Count() > 0)
						canPlaceKey = true;

					if (!canPlaceKey)
						continue;

					possibleSpawnTiles.Add(t);
				}

				var possibleSpawnComponents = possibleSpawnTiles.SelectMany(x => x.GetComponentsInChildren<Component>().OfType<IKeySpawnable>()).ToList();

				if (possibleSpawnComponents.Count == 0)
					locksToRemove.Add(door);
				else
				{
					int keysToSpawn = key.KeysPerLock.GetRandom(RandomStream);
					keysToSpawn = Math.Min(keysToSpawn, possibleSpawnComponents.Count);

					for (int i = 0; i < keysToSpawn; i++)
					{
						int chosenCompID = RandomStream.Next(0, possibleSpawnComponents.Count);
						var comp = possibleSpawnComponents[chosenCompID];
						comp.SpawnKey(key, DungeonFlow.KeyManager);

						foreach (var k in (comp as Component).GetComponentsInChildren<Component>().OfType<IKeyLock>())
							k.OnKeyAssigned(key, DungeonFlow.KeyManager);

						possibleSpawnComponents.RemoveAt(chosenCompID);
					}
				}
			}

			foreach (var door in locksToRemove)
				lockedDoorways.Remove(door);

			foreach (var pair in lockedDoorways)
			{
				pair.Key.RemoveUsedPrefab();
				LockDoorway(pair.Key, pair.Value, DungeonFlow.KeyManager);
			}
		}

		protected virtual void LockDoorway(Doorway doorway, Key key, KeyManager keyManager)
		{
			var placement = doorway.Tile.Placement;
			var prefabs = doorway.Tile.TileSet.LockPrefabs.Where(x =>
			{
				var lockSocket = x.Socket;

				if (lockSocket == null)
					return true;
				else
					return DoorwaySocket.CanSocketsConnect(lockSocket, doorway.Socket);
			}).Select(x => x.LockPrefabs).ToArray();

			if (prefabs.Length == 0)
				return;

			var chosenEntry = prefabs[RandomStream.Next(0, prefabs.Length)].GetRandom(RandomStream, placement.IsOnMainPath, placement.NormalizedDepth, null, true);
			var prefab = chosenEntry.Value;

			GameObject doorObj = GameObject.Instantiate(prefab, doorway.transform);

			DungeonUtil.AddAndSetupDoorComponent(CurrentDungeon, doorObj, doorway);

			// Set this locked door as the current door prefab
			doorway.SetUsedPrefab(doorObj);
			doorway.ConnectedDoorway.SetUsedPrefab(doorObj);

			foreach (var keylock in doorObj.GetComponentsInChildren<Component>().OfType<IKeyLock>())
				keylock.OnKeyAssigned(key, keyManager);
		}

		#region ISerializationCallbackReceiver Implementation

		public void OnBeforeSerialize()
		{
			fileVersion = CurrentFileVersion;
		}

		public void OnAfterDeserialize()
		{
			if (fileVersion < 1)
				RepeatMode = (allowImmediateRepeats) ? TileRepeatMode.Allow : TileRepeatMode.DisallowImmediate;
		}

		#endregion
	}
}
