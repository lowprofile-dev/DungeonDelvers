using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using SkredUtils;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public abstract class EquippableBase : ItemBase
{
    private void Reset()
    {
        Tiers.Add(EquippableTier.Damaged, new EquippableTierRewards {BonusEnhancementSlots = 0});
        Tiers.Add(EquippableTier.Normal, new EquippableTierRewards {BonusEnhancementSlots = 1});
        Tiers.Add(EquippableTier.Rare, new EquippableTierRewards {BonusEnhancementSlots = 2});
        Tiers.Add(EquippableTier.Epic, new EquippableTierRewards {BonusEnhancementSlots = 3});
        Tiers.Add(EquippableTier.Masterpiece, new EquippableTierRewards {BonusEnhancementSlots = 4});
    }

    public int EnhancementSlots = 0;
    [FoldoutGroup("Enhancement Stats")]
    public Stats EnhancementStats;
    public int EnhancementBaseCost;
    
    public List<PlayerSkill> Skills;
    public List<Passive> Passives;

    public PlayerSkill[] GetPlayerSkills(EquippableTier tier) 
        => Skills.Concat(Tiers[tier].PlayerSkills).ToArray();
    
    
    public Passive[] GetPassives(EquippableTier tier)
        => Passives.Concat(Tiers[tier].Passives).ToArray();
    
    
    public abstract EquippableSlot Slot { get; }

    [FormerlySerializedAs("EquippableTierRewards")] public Dictionary<EquippableTier, EquippableTierRewards> Tiers 
        = new Dictionary<EquippableTier, EquippableTierRewards>();

    public Dictionary<EquippableTier, int> CustomTierWeights = null;

    public EquippableTier GetRandomTier()
    {
        var availableTiers = Tiers.Keys.ToArray();
        var weights = CustomTierWeights ?? GameSettings.Instance.DefaultEquipmentQualityChance;
        return availableTiers.WeightedRandom(t => weights[t]);
    }
    
    #region Declarations

    [Serializable] public enum EquippableSlot
    {
        Accessory,
        Body,
        Feet,
        Hand,
        Head,
        Weapon
    }

    [Serializable] public enum ArmorType
    {
        Light,
        Heavy
    }

    [Serializable] public enum EquippableTier
    {
        Damaged = 0,
        Normal = 1,
        Rare = 2,
        Epic = 3,
        Masterpiece = 4
    }

    #endregion
}

public class EquippableTierRewards
{
    public int BonusEnhancementSlots = 0;
    public List<PlayerSkill> PlayerSkills = new List<PlayerSkill>();
    public List<Passive> Passives = new List<Passive>();
    public Stats TierStats;
}