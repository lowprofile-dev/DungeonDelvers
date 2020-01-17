using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DunGen
{
    public sealed class DungeonGraphNode : DungeonGraphObject
    {
        public List<DungeonGraphConnection> Connections = new List<DungeonGraphConnection>();
        public Tile Tile { get; private set; }


        public DungeonGraphNode(Tile tile)
        {
            Tile = tile;
        }

        internal void AddConnection(DungeonGraphConnection connection)
        {
            Connections.Add(connection);
        }
    }
}
