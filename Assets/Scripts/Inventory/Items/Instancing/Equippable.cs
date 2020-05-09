using System;
using Sirenix.OdinInspector;
using SkredUtils;

public class Equippable : Item
{
    public override ItemSave Serialize()
    {
        return new EquippableSave
        {
            baseUid = GameSettings.Instance.ItemDatabase.GetId(Base).Value,
            EnhancementCount = EnhancementCount,
            Tier = (int)Tier
        };
    }
    
    public EquippableBase EquippableBase => Base as EquippableBase;
    [ShowIf("hasBase")] public EquippableBase.EquippableSlot Slot => EquippableBase.Slot;
    
    #region Stats

    public PlayerSkill[] GetSkills => EquippableBase.GetPlayerSkills(Tier);
    public Passive[] GetPassives => EquippableBase.GetPassives(Tier);
    public Stats GetStats => EquippableBase.Tiers[Tier].TierStats + EquippableBase.EnhancementStats*EnhancementCount;
    public int EnhancementCount = 0;
    public EquippableBase.EquippableTier Tier = EquippableBase.EquippableTier.Normal;

    #endregion
    
    #region Instancing

    public Equippable(EquippableBase equippableBase) : base(equippableBase)
    {
        EnhancementCount = 0;
    }

    public Equippable(EquippableSave equippableSave) : base(equippableSave)
    {
        Tier = (EquippableBase.EquippableTier) equippableSave.Tier;
        EnhancementCount = equippableSave.EnhancementCount;
    }

    #endregion

    #region Functions

    public void Enhance() => EnhancementCount++;

    public int GetEnhancementCost => (EnhancementCount + 1) * EquippableBase.EnhancementBaseCost;

    public int GetEnhancementSlots => EquippableBase.EnhancementSlots + EquippableBase.Tiers[Tier].BonusEnhancementSlots;
    
    public void Reforge()
    {
        Tier = EquippableBase.GetRandomTier();
        EnhancementCount = 0;
    }

    #endregion
    
    #region Info

    public override string InspectorName =>
        Base.itemName
        + (EnhancementCount > 0
            ? $" +{EnhancementCount}"
            : "");

    public override string ColoredInspectorName =>
        $"<color={GameSettings.Instance.DefaultEquipmentQualityColor[Tier].ToHex()}>{InspectorName}</color>";

    public string TierQualifiedName =>
        Tier == EquippableBase.EquippableTier.Normal
            ? Base.itemName
            : $"<color={GameSettings.Instance.DefaultEquipmentQualityColor[Tier].ToHex()}>{Tier.ToString()} {Base.itemName}</color>";
    
    public string SlotName
    {
        get
        {
            var equippableBase = EquippableBase;
            if (equippableBase is WeaponBase weaponBase) return weaponBase.weaponType.ToString();
            if (equippableBase is BodyBase bodyBase) return bodyBase.ArmorType.ToString();
            return equippableBase.Slot.ToString();
        }
    }

    public override string InspectorDescription =>
        $"Equipment Type: {SlotName}\nRarity: <color={GameSettings.Instance.DefaultEquipmentQualityColor[Tier].ToHex()}>{Tier.ToString()}</color>\n{Base.itemText}";

    public string StatsDescription
    {
        get
        {
            var str = "";
            var stats = GetStats;
            if (stats.MaxHp > 0) str += $"Max HP: +{stats.MaxHp}\n";
            if (stats.PhysAtk > 0) str += $"Phys. Attack: +{stats.PhysAtk}\n";
            if (stats.MagAtk > 0) str += $"Mag. Attack: +{stats.MagAtk}\n";
            if (stats.PhysDef > 0) str += $"Phys. Defense: +{stats.PhysDef}\n";
            if (stats.MagDef > 0) str += $"Mag. Defense: +{stats.MagDef}\n";
            if (stats.Speed > 0) str += $"Speed: +{stats.Speed}\n";
            if (stats.Accuracy > 0) str += $"Accuracy: +{stats.Accuracy:F3}\n";
            if (stats.Evasion > 0) str += $"Evasion: +{stats.Evasion:F3}\n";
            if (stats.CritChance > 0) str += $"Crit. Chance: +{stats.CritChance:F3}\n";
            if (stats.CritAvoid > 0) str += $"Crit. Avoid: +{stats.CritAvoid:F3}\n";
            return str;
        }
    }

    public string EnhancedStatsDescription
    {
        get
        {
            EnhancementCount++;
            var str = StatsDescription;
            EnhancementCount--;
            return str;
        }
    }
    
    #endregion

#if UNITY_EDITOR
    private bool hasBase() => Base != null;
#endif
}