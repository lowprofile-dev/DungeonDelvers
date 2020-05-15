using System;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using XNode;

[Serializable]
public class MasteryInstance
{
    [SerializeField, ReadOnly] private Character owner;
    [ShowInInspector] public MasteryGraph Graph { get; private set; }
    [ShowInInspector] public MasteryNode Node { get; private set; }
    [ShowInInspector] public MasteryIndex Id { get; private set; }
    [ShowInInspector] public int Level { get; private set; }

    public MasteryInstance(Character owner, int graphId, MasteryNode masteryNode)
    {
        this.owner = owner;
        Graph = (MasteryGraph) masteryNode.graph;
        Node = masteryNode;
        Id = new MasteryIndex(graphId, masteryNode.Id);
        Level = masteryNode.AutoLearned ? masteryNode.MasteryMaxLevel : 0;
    }

    public MasteryInstance(Character owner, SerializedMasteryInstance serializedMasteryInstance)
    {
        this.owner = owner;
        Graph = owner.Base.Masteries[serializedMasteryInstance.Id.GraphId];
        Node = (MasteryNode) Graph.nodes[serializedMasteryInstance.Id.NodeId];
        Id = serializedMasteryInstance.Id;
        Level = serializedMasteryInstance.Level;
    }

    public SerializedMasteryInstance Serialize() => new SerializedMasteryInstance {Id = Id, Level = Level};

    [ShowInInspector, TabGroup("Conditions")] public bool Unlocked => Node.GetPrerequisites().All(pR => pR.PrerequisiteAchieved(owner));
    [ShowInInspector, TabGroup("Conditions")] public bool Maxed => Node.MasteryMaxLevel <= Level;
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

[Serializable] public struct MasteryIndex
{
    public int GraphId;
    public int NodeId;

    public MasteryIndex(int gid, int nid)
    {
        GraphId = gid;
        NodeId = nid;
    }
}

[Serializable] public struct SerializedMasteryInstance
{
    public MasteryIndex Id;
    public int Level;
}
