using System;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Monster")]
public class Monster : SerializableAsset
{
    public string MonsterName;
    public GameObject MonsterBattler;
    public int BaseLevel;
    public int LevelVariance;
    public Stats Stats;
    public Stats StatLevelVariance;
    public List<MonsterSkill> Skills = new List<MonsterSkill>();
    public List<Passive> Passives = new List<Passive>();
    public MonsterAI MonsterAi;
}