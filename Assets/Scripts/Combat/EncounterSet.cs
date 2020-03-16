using System;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "EncounterSet")]
public class EncounterSet : SerializableAsset {
    public List<EncounterMonster> EncounterMonsters = new List<EncounterMonster>();
    public EncounterLayout Layout;

    public List<MonsterBattler> LoadMonstersInto(RectTransform encounterBase){
        var monsters = new List<MonsterBattler>();

        foreach(var encounterMonster in EncounterMonsters){
            var monsterBattler = Instantiate(encounterMonster.Monster.MonsterBattler, encounterBase);
            var rect = monsterBattler.GetComponent<RectTransform>();
            rect.transform.rotation = Quaternion.Euler(50,0,0);
            var monster = monsterBattler.AddComponent<MonsterBattler>();
            monster.LoadBase(encounterMonster.Monster);
        }
    }

    public enum EncounterLayout {
        ZigZag
    }
}

[Serializable]
public struct EncounterMonster{
    public Monster Monster;
    public int MinLevel;
    public int MaxLevel;
}