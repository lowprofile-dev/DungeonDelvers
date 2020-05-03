using System;
using UnityEngine;
using XNode;

[Serializable]
public class MasteryInstance
{
    public MasteryGraph Graph { get; private set; }
    public MasteryNode Node { get; private set; }
    public int Id { get; private set; }
    private int level;
    public int Level
    {
        get => level;
        set { level = Mathf.Min(Node.MasteryMaxLevel, value); }
    }

    public MasteryInstance(MasteryNode masteryNode)
    {
        Graph = (MasteryGraph) masteryNode.graph;
        Node = masteryNode;
        Id = masteryNode.Id;
        Level = 0;
    }

    public MasteryInstance(MasteryGraph graph, SerializedMasteryInstance serializedMasteryInstance)
    {
        Graph = graph;
        Node = (MasteryNode) graph.nodes[serializedMasteryInstance.Id];
        Id = serializedMasteryInstance.Id;
        Level = serializedMasteryInstance.Level;
    }
}

public struct SerializedMasteryInstance
{
    public int Id;
    public int Level;
}
