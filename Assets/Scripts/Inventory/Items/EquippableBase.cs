using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;

public abstract class EquippableBase : ItemBase
{
    [FoldoutGroup("Stats")] public Stats Stats;
    public List<PlayerSkill> Skills;
    public List<Passive> Passives;

    public enum EquippableSlot
    {
        Accessory,
        Body,
        Feet,
        Hand,
        Head,
        Weapon
    }

    public enum ArmorType
    {
        Light,
        Heavy
    }
    
    public abstract EquippableSlot Slot { get; }
}