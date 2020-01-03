using System;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Monster"), Obsolete]
public class Monster : ScriptableObject
{
    public string Name;
    public GameObject MonsterBattler;

    [FoldoutGroup("Stats")] public int MaxHp;
    [FoldoutGroup("Stats")] public int InitialEp;
    [FoldoutGroup("Stats")] public int EpGain;
    [FoldoutGroup("Stats")] public int PhysAtk;
    [FoldoutGroup("Stats")] public int MagAtk;
    [FoldoutGroup("Stats")] public int MagDef;
    [FoldoutGroup("Stats")] public int Speed;
    [FoldoutGroup("Stats")] public float Accuracy;
    [FoldoutGroup("Stats")] public float Evasion;
    [FoldoutGroup("Stats")] public float CritChance;
    [FoldoutGroup("Stats")] public float CritAvoid;
}
