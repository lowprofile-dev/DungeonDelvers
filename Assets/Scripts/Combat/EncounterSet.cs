using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(menuName = "EncounterSet")]
public class EncounterSet : SerializableAsset {
    public List<EncounterMonster> EncounterMonsters = new List<EncounterMonster>();
    public EncounterLayout Layout;
    public int? overrideExpGain;
    public int? overrideGoldGain;
    public List<ItemBase> overrideItemDrops; //Refazer pra permitir consumivel futuramente, usando IMonsterDrop(?)
    
    public List<MonsterBattler> BuildMonsters()
    {
        var battlers = new List<MonsterBattler>();

        foreach (var encounterMonster in EncounterMonsters)
        {
            var battler = new GameObject(encounterMonster.Monster.MonsterName);
            battler.AddComponent<RectTransform>();
            var monsterBattler = battler.AddComponent<MonsterBattler>();
            monsterBattler.LoadEncounterMonster(encounterMonster);
            battlers.Add(monsterBattler);
            // var battler = Instantiate(encounterMonster.Monster.MonsterBattler);
            // var monsterBattler = battler.AddComponent<MonsterBattler>();
            // monsterBattler.LoadEncounterMonster(encounterMonster);
            // battlers.Add(monsterBattler);
        }

        return battlers;
    }

    public int GetGoldReward()
    {
        if (overrideGoldGain.HasValue)
            return overrideGoldGain.Value;

        return EncounterMonsters.Sum(encounterMonster => encounterMonster.Monster.RollGold());
    }

    public IList<Item> GetItemReward()
    {
        if (overrideItemDrops != null)
            return overrideItemDrops.Select(ItemInstanceBuilder.BuildInstance).ToList();

        var list = new List<Item>();

        foreach (var encounterMonster in EncounterMonsters)
        {
            var monsterDrop = encounterMonster.Monster.RollItems();
            list.AddRange(monsterDrop);
        }

        return list;
    }
    
    public enum EncounterLayout {
        ZigZag,
        Line
    }
}

[Serializable]
public struct EncounterMonster{
    public Monster Monster;
    public int MinLevel;
    public int MaxLevel;
}