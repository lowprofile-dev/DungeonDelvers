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
            Tier = (int)Tier,
        };
    }
    
    public EquippableBase EquippableBase => Base as EquippableBase;
    [ShowIf("hasBase")] public EquippableBase.EquippableSlot Slot => EquippableBase.Slot;
    
    #region Stats

    public PlayerSkill[] GetSkills => EquippableBase.GetPlayerSkills(Tier);
    public Passive[] GetPassives => EquippableBase.GetPassives(Tier);
    public Stats GetStats => EquippableBase.Tiers[Tier].TierStats;
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
    }

    #endregion

    #region Functions

    public void Enhance() => EnhancementCount++;

    public int GetEnhancementCost() => (EnhancementCount + 1) * EquippableBase.EnhancementBaseCost;
    
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

    #endregion

#if UNITY_EDITOR
    private bool hasBase() => Base != null;
#endif
}