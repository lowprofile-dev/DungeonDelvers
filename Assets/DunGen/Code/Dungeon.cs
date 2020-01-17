using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using DunGen.Graph;
using DunGen.Adapters;

namespace DunGen
{
	public class Dungeon : MonoBehaviour
	{
		public Bounds Bounds { get; protected set; }
		public DungeonFlow DungeonFlow { get; protected set; }
        public bool DebugRender = false;

        public ReadOnlyCollection<Tile> AllTiles { get; private set; }
        public ReadOnlyCollection<Tile> MainPathTiles { get; private set; }
        public ReadOnlyCollection<Tile> BranchPathTiles { get; private set; }
        public ReadOnlyCollection<GameObject> Doors { get; private set; }
        public ReadOnlyCollection<DoorwayConnection> Connections { get; private set; }
        public DungeonGraph ConnectionGraph { get; private set; }

		private readonly List<Tile> allTiles = new List<Tile>();
        private readonly List<Tile> mainPathTiles = new List<Tile>();
        private readonly List<Tile> branchPathTiles = new List<Tile>();
        private readonly List<GameObject> doors = new List<GameObject>();
        private readonly List<DoorwayConnection> connections = new List<DoorwayConnection>();


        public Dungeon()
        {
            AllTiles = new ReadOnlyCollection<Tile>(allTiles);
            MainPathTiles = new ReadOnlyCollection<Tile>(mainPathTiles);
            BranchPathTiles = new ReadOnlyCollection<Tile>(branchPathTiles);
            Doors = new ReadOnlyCollection<GameObject>(doors);
            Connections = new ReadOnlyCollection<DoorwayConnection>(connections);
        }

		internal void AddAdditionalDoor(Door door)
		{
			if(door != null)
				doors.Add(door.gameObject);
		}

		internal void PreGenerateDungeon(DungeonGenerator dungeonGenerator)
		{
			DungeonFlow = dungeonGenerator.DungeonFlow;
		}

        internal void PostGenerateDungeon(DungeonGenerator dungeonGenerator)
        {
            ConnectionGraph = new DungeonGraph(this);
			Bounds = UnityUtil.CombineBounds(allTiles.Select(x => x.Placement.Bounds).ToArray());
        }

		public void Clear()
		{
			// Destroy all tiles
			foreach (var tile in allTiles)
			{
				// Clean up any door prefabs first
				foreach (var doorway in tile.Placement.UsedDoorways)
				{
					if (doorway.UsedDoorPrefab != null)
						UnityUtil.Destroy(doorway.UsedDoorPrefab);
				}

				UnityUtil.Destroy(tile.gameObject);
			}

			// Destroy anything else attached to this dungeon
			for (int i = 0; i < transform.childCount; i++)
			{
				GameObject child = transform.GetChild(i).gameObject;
				UnityUtil.Destroy(child);
			}

			allTiles.Clear();
			mainPathTiles.Clear();
			branchPathTiles.Clear();
            doors.Clear();
            connections.Clear();
		}

        public Doorway GetConnection(Doorway doorway)
        {
            foreach (var conn in connections)
                if (conn.A == doorway)
                    return conn.B;
                else if (conn.B == doorway)
                    return conn.A;

            return null;
        }

		internal void MakeConnection(Doorway a, Doorway b, System.Random randomStream)
		{
			bool areDoorwaysFromDifferentDungeons = (a.Dungeon != b.Dungeon);

			a.Tile.Placement.UnusedDoorways.Remove(a);
			a.Tile.Placement.UsedDoorways.Add(a);

			b.Tile.Placement.UnusedDoorways.Remove(b);
			b.Tile.Placement.UsedDoorways.Add(b);

			a.ConnectedDoorway = b;
			b.ConnectedDoorway = a;

			if (!areDoorwaysFromDifferentDungeons)
			{
				var conn = new DoorwayConnection(a, b);
				connections.Add(conn);
			}

			// Add door prefab
			Doorway chosenDoor;

			// If both doorways have door prefabs..
			if (a.DoorPrefabs.Count > 0 && b.DoorPrefabs.Count > 0)
			{
				// ..A is selected if its priority is greater than or equal to B..
				if (a.DoorPrefabPriority >= b.DoorPrefabPriority)
					chosenDoor = a;
				// .. otherwise, B is chosen..
				else
					chosenDoor = b;
			}
			// ..if only one doorway has a prefab, use that one
			else
				chosenDoor = (a.DoorPrefabs.Count > 0) ? a : b;


			List<GameObject> doorPrefabs = chosenDoor.DoorPrefabs;

			if (doorPrefabs.Count > 0 && !(a.HasDoorPrefab || b.HasDoorPrefab))
			{
				GameObject doorPrefab = doorPrefabs[randomStream.Next(0, doorPrefabs.Count)];

				if (doorPrefab != null)
				{
					GameObject door = Instantiate(doorPrefab, chosenDoor.transform);
                    door.transform.localPosition = Vector3.zero;

                    if (!chosenDoor.AvoidRotatingDoorPrefab)
                        door.transform.localRotation = Quaternion.identity;

					doors.Add(door);

					DungeonUtil.AddAndSetupDoorComponent(this, door, chosenDoor);

					a.SetUsedPrefab(door);
					b.SetUsedPrefab(door);
				}
			}
		}

		internal void AddTile(Tile tile)
        {
            allTiles.Add(tile);

            if (tile.Placement.IsOnMainPath)
                mainPathTiles.Add(tile);
            else
                branchPathTiles.Add(tile);
        }

        internal void RemoveTile(Tile tile)
        {
            allTiles.Remove(tile);

            if (tile.Placement.IsOnMainPath)
                mainPathTiles.Remove(tile);
            else
                branchPathTiles.Remove(tile);
        }

        internal void RemoveLastConnection()
        {
            var conn = connections.Last();

            Doorway a = conn.A;
            Doorway b = conn.B;

            a.Tile.Placement.UnusedDoorways.Add(a);
            a.Tile.Placement.UsedDoorways.Remove(a);

            b.Tile.Placement.UnusedDoorways.Add(b);
            b.Tile.Placement.UsedDoorways.Remove(b);

            a.ConnectedDoorway = null;
            b.ConnectedDoorway = null;

            // Remove door prefabs if any were placed
            if (a.HasDoorPrefab)
            {
                doors.Remove(a.UsedDoorPrefab);
                a.RemoveUsedPrefab();
            }
            if (b.HasDoorPrefab)
            {
                doors.Remove(b.UsedDoorPrefab);
                b.RemoveUsedPrefab();
            }

            connections.Remove(conn);
        }

        public void OnDrawGizmos()
        {
            if (DebugRender)
                DebugDraw();
        }

        public void DebugDraw()
        {
            Color mainPathStartColour = Color.red;
            Color mainPathEndColour = Color.green;
            Color branchPathStartColour = Color.blue;
            Color branchPathEndColour = new Color(0.5f, 0, 0.5f);
            float boundsBoxOpacity = 0.75f;

            foreach (var tile in allTiles)
            {
                Bounds bounds = tile.Placement.Bounds;
                bounds.size = bounds.size * 1.01f;

                Color tileColour = (tile.Placement.IsOnMainPath) ?
                                    Color.Lerp(mainPathStartColour, mainPathEndColour, tile.Placement.NormalizedDepth) :
                                    Color.Lerp(branchPathStartColour, branchPathEndColour, tile.Placement.NormalizedDepth);

                tileColour.a = boundsBoxOpacity;
                Gizmos.color = tileColour;

                Gizmos.DrawCube(bounds.center, bounds.size);

            }
        }
	}
}
