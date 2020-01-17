using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DunGen.Graph
{
    [Serializable]
	public abstract class FlowGraphObjectReference
	{
        public DungeonFlow Flow { get { return flow; } }

        [SerializeField]
        protected DungeonFlow flow;
        [SerializeField]
        protected int index;
	}

    [Serializable]
    public sealed class FlowNodeReference : FlowGraphObjectReference
    {
        public GraphNode Node
        {
            get { return flow.Nodes[index]; }
            set { index = flow.Nodes.IndexOf(value); }
        }

        public FlowNodeReference(DungeonFlow flowGraph, GraphNode node)
        {
            flow = flowGraph;
            Node = node;
        }
    }

    [Serializable]
    public sealed class FlowLineReference : FlowGraphObjectReference
    {
        public GraphLine Line
        {
            get { return flow.Lines[index]; }
			set { index = flow.Lines.IndexOf(value); }
        }

        public FlowLineReference(DungeonFlow flowGraph, GraphLine line)
        {
            flow = flowGraph;
            Line = line;
        }
    }
}
