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
    public Stats Stats;
    public List<MonsterSkill> Skills = new List<MonsterSkill>();
    public MonsterAI MonsterAi;
}
