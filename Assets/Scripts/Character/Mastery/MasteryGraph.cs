using System;
using System.Collections.Generic;
using XNode;
using System.Linq;
using Sirenix.Utilities;
using UnityEngine;

[CreateAssetMenu(menuName = "Mastery Graph")]
public class MasteryGraph : NodeGraph
{
    public List<MasteryInstance> Instances = new List<MasteryInstance>();

    public void Initialize()
    {
        Instances.Clear();
        foreach (MasteryNode node in nodes)
        {
            Instances.Add(new MasteryInstance(node));
        }
    }

    public void Initialize(SerializedMasteryInstance[] save)
    {
        Instances.Clear();
        if (save.Length != nodes.Count)
            throw new DeserializationFailureException(typeof(MasteryInstance));

        foreach (var serializedMasteryInstance in save)
        {
            Instances.Add(new MasteryInstance(this, serializedMasteryInstance));
        }
    }

    public void ApplyMasteries(Character character) => nodes.Cast<MasteryNode>().ForEach(node => node.ApplyEffects(character));
}
