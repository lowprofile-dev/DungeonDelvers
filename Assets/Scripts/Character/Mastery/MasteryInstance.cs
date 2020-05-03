using System;
using System.Linq;
using UnityEngine;
using XNode;

[Serializable]
public class MasteryInstance
{
    private Character owner;
    public MasteryGraph Graph { get; private set; }
    public MasteryNode Node { get; private set; }
    public int Id { get; private set; }
    public int Level { get; private set; }

    public MasteryInstance(Character owner, MasteryNode masteryNode)
    {
        this.owner = owner;
        Graph = (MasteryGraph) masteryNode.graph;
        Node = masteryNode;
        Id = masteryNode.Id;
        Level = 0;
    }

    public MasteryInstance(Character owner, MasteryGraph graph, SerializedMasteryInstance serializedMasteryInstance)
    {
        this.owner = owner;
        Graph = graph;
        Node = (MasteryNode) graph.nodes[serializedMasteryInstance.Id];
        Id = serializedMasteryInstance.Id;
        Level = serializedMasteryInstance.Level;
    }

    public SerializedMasteryInstance Serialize() => new SerializedMasteryInstance {Id = Id, Level = Level};

    public bool Unlocked => Node.GetPrerequisites().All(pR => pR.PrerequisiteAchieved(owner));
    public bool Maxed => Node.MasteryMaxLevel <= Level;
    public bool Available => Unlocked && !Maxed;

    public void ApplyEffects() =>
        Node.MasteryEffects.ForEach(mE => mE.ApplyEffect(owner, Level));

    public void LevelUp()
    {
        if (Level < Node.MasteryMaxLevel && owner.CurrentMp >= Node.MasteryPointCost)
        {
            Level++;
            owner.CurrentMp -= Node.MasteryPointCost;
            owner.Regenerate();
        }
    }
}

public struct SerializedMasteryInstance
{
    public int Id;
    public int Level;
}
