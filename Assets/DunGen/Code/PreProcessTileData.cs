using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace DunGen
{
	public sealed class PreProcessTileData
	{
		public static Type ProBuilderMeshType { get; private set; }
		public static PropertyInfo ProBuilderPositionsProperty { get; private set; }


		public GameObject Prefab { get; private set; }
		public Tile Tile { get; private set; }
		public GameObject Proxy { get; private set; }
		public readonly List<GameObject> ProxySockets = new List<GameObject>();

		public readonly List<DoorwaySocket> DoorwaySockets = new List<DoorwaySocket>();
		public readonly List<Doorway> Doorways = new List<Doorway>();


		#region Static Methods

		static PreProcessTileData()
		{
			FindProBuilderObjectType();
		}

		public static void FindProBuilderObjectType()
		{
			if (ProBuilderMeshType != null)
				return;

			// Look through each of the loaded assemblies in our current AppDomain, looking for ProBuilder's pb_Object type
			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				if (assembly.FullName.Contains("ProBuilder"))
				{
					ProBuilderMeshType = assembly.GetType("UnityEngine.ProBuilder.ProBuilderMesh");

					if (ProBuilderMeshType != null)
					{
						ProBuilderPositionsProperty = ProBuilderMeshType.GetProperty("positions");

						if (ProBuilderPositionsProperty != null)
							break;
					}
				}
			}
		}

		#endregion


		public PreProcessTileData(GameObject prefab, bool ignoreSpriteRendererBounds, Vector3 upVector)
		{
			Prefab = prefab;
			Tile = prefab.GetComponent<Tile>();
			Proxy = new GameObject(prefab.name + "_PROXY");

			// Reset prefab transforms
			prefab.transform.position = Vector3.zero;
			prefab.transform.rotation = Quaternion.identity;

			GetAllDoorways();

			// Copy all doors to the proxy object
			foreach (var door in Doorways)
			{
				var proxyDoor = new GameObject("ProxyDoor");
				proxyDoor.transform.position = door.transform.position;
				proxyDoor.transform.rotation = door.transform.rotation;

				proxyDoor.transform.parent = Proxy.transform;
				ProxySockets.Add(proxyDoor);
			}

			Bounds bounds;

			if (Tile != null && Tile.OverrideAutomaticTileBounds)
				bounds = Tile.TileBoundsOverride;
			else
				bounds = CalculateProxyBounds(ignoreSpriteRendererBounds, upVector);

			// Let the user know if the automatically calculated bounds are incorrect
			if (bounds.size.x <= 0f || bounds.size.y <= 0f || bounds.size.z <= 0f)
				Debug.LogError(string.Format("Tile prefab '{0}' has automatic bounds that are zero or negative in size. The bounding volume for this tile will need to be manually defined.", prefab), prefab);

			bounds = UnityUtil.CondenseBounds(bounds, Prefab.GetComponentsInChildren<Doorway>());

			var collider = Proxy.AddComponent<BoxCollider>();
			collider.center = bounds.center;
			collider.size = bounds.size;
		}

		private Bounds CalculateProxyBounds(bool ignoreSpriteRendererBounds, Vector3 upVector)
		{
			var bounds = UnityUtil.CalculateObjectBounds(Prefab, true, ignoreSpriteRendererBounds);

			// Since ProBuilder objects don't have a mesh until they're instantiated, we have to calculate the bounds manually
			if (ProBuilderMeshType != null && ProBuilderPositionsProperty != null)
			{
				foreach (var pbMesh in Prefab.GetComponentsInChildren(ProBuilderMeshType))
				{
					Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
					Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

					var vertices = (IList<Vector3>)ProBuilderPositionsProperty.GetValue(pbMesh, null);

					foreach (var vert in vertices)
					{
						min = Vector3.Min(min, vert);
						max = Vector3.Max(max, vert);
					}

					Vector3 size = Prefab.transform.TransformDirection(max - min);
					Vector3 center = Prefab.transform.TransformPoint(min) + size / 2;

					bounds.Encapsulate(new Bounds(center, size));
				}
			}

			return bounds;
		}

		private void GetAllDoorways()
		{
			DoorwaySockets.Clear();

			foreach (var d in Prefab.GetComponentsInChildren<Doorway>())
			{
				Doorways.Add(d);

				if (!DoorwaySockets.Contains(d.Socket))
					DoorwaySockets.Add(d.Socket);
			}
		}
	}
}
