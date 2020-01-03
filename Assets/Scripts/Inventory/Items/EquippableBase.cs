using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;

public abstract class EquippableBase : ItemBase
{
    [FoldoutGroup("Stats")] public Stats Stats;
    public List<Skill> Skills;

    public enum EquippableSlot
    {
        Accessory,
        Body,
        Feet,
        Hand,
        Head,
        Weapon
    }
    
    public abstract EquippableSlot Slot { get; }
}