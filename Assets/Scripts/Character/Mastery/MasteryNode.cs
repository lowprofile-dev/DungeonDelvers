using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities;
using UnityEngine;
using XNode;

public class MasteryNode : Node, IMasteryPrerequisite
{
    [ShowInInspector,PropertyOrder(-100)] public int Id => graph.nodes.IndexOf(this);
    public string MasteryName;
    public Sprite MasterySprite;
    [TextArea] public string MasteryDescription;
    [OdinSerialize] public List<MasteryEffect> MasteryEffects;
    public int MasteryMaxLevel = 1;
    public int MasteryPointCost = 1;
    private MasteryInstance Instance => (graph as MasteryGraph).Instances[Id];
    [Input(ShowBackingValue.Never)] public MasteryNode Prerequisites;

    public MasteryNode[] GetPrerequisites() => GetInputValues("Prerequisites", Prerequisites);

    public bool Maxed => Instance.Level >= MasteryMaxLevel;
    public bool PrerequisiteAchieved => Maxed;
    public bool Available => GetPrerequisites().All(req => req.Maxed);

    public void ApplyEffects(Character character) =>
        MasteryEffects.ForEach(mE => mE.ApplyEffect(character, Instance.Level));
}
