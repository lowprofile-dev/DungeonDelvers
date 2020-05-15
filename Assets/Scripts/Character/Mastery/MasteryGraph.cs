using System;
using System.Collections.Generic;
using XNode;
using System.Linq;
using Sirenix.Utilities;
using UnityEngine;

[CreateAssetMenu(menuName = "Mastery Graph")]
public class MasteryGraph : NodeGraph
{
    public List<MasteryInstance> Initialize(Character owner, int graphIndex)
    {
        var instances = new List<MasteryInstance>();
        foreach (var node in nodes) 
             if (node is MasteryNode masteryNode) 
                 instances.Add(new MasteryInstance(owner, graphIndex, masteryNode));
        return instances;
    }

    public static List<MasteryInstance> Initialize(Character owner, SerializedMasteryInstance[] save)
    {
        var instances = new List<MasteryInstance>();

        foreach (var serializedMasteryInstance in save)
        {
            instances.Add(new MasteryInstance(owner, serializedMasteryInstance));
        }
        return instances;
    }

    public override Node AddNode(Type type)
    {
        var node = base.AddNode(type);
        for (var i = 0; i < nodes.Count; i++) if (nodes[i] is MasteryNode masteryNode) masteryNode.Id = i;
        return node;
    }

    public override Node CopyNode(Node original)
    {
        var node = base.CopyNode(original);
        for (var i = 0; i < nodes.Count; i++) ((MasteryNode) nodes[i]).Id = i;
        return node;
    }
}
