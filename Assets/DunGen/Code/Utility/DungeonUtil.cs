using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DunGen
{
    public static class DungeonUtil
    {
        /// <summary>
        /// Appends one dungeon to another
        /// NOTE: This is NOT supported functionality; the dungeons will likely overlap, it's up to you to decide how/if you're going to handle
        /// that situation. Use of portal culling such as that provided by SECTR VIS will help to hide the overlap.
        /// </summary>
        /// <param name="previousDungeon">The dungeon to append to</param>
        /// <param name="newDungeon">The dungeon to append</param>
        /// <param name="randomStream">A random number generator to be passed in (from the DungeonGenerator)</param>
        public static void Append(Dungeon previousDungeon, Dungeon newDungeon, System.Random randomStream)
        {
            Doorway doorwayA = previousDungeon.MainPathTiles[previousDungeon.MainPathTiles.Count - 1].Placement.UnusedDoorways[0];
            Doorway doorwayB = newDungeon.MainPathTiles[0].Placement.UnusedDoorways[0];

            UnityUtil.PositionObjectBySocket(previousDungeon.gameObject, doorwayA.gameObject, doorwayB.gameObject);

            newDungeon.MakeConnection(doorwayA, doorwayB, randomStream);

            // Remove objects that should only exist when the door is not in use (NOTE: We can't get back any object from the AddWhenInUse list)
            foreach (var obj in doorwayA.AddWhenNotInUse)
                UnityUtil.Destroy(obj);
            foreach (var obj in doorwayB.AddWhenNotInUse)
                UnityUtil.Destroy(obj);
        }

		/// <summary>
		/// Adds a Door component to the selected doorPrefab if one doesn't already exist
		/// </summary>
		/// <param name="dungeon">The dungeon that this door belongs to</param>
		/// <param name="doorPrefab">The door prefab on which to apply the component</param>
		/// <param name="doorway">The doorway that this door belongs to</param>
		public static void AddAndSetupDoorComponent(Dungeon dungeon, GameObject doorPrefab, Doorway doorway)
		{
			var door = doorPrefab.GetComponent<Door>();

			if (door == null)
				door = doorPrefab.AddComponent<Door>();

			door.Dungeon = dungeon;
			door.DoorwayA = doorway;
			door.DoorwayB = doorway.ConnectedDoorway;
			door.TileA = doorway.Tile;
			door.TileB = doorway.ConnectedDoorway.Tile;

			dungeon.AddAdditionalDoor(door);
		}
	}
}
