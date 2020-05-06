using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public abstract class EquippableBase : ItemBase
{
    private void Reset()
    {
        
    }
    
    [FoldoutGroup("Stats")] public Stats BaseStats;
    public List<PlayerSkill> Skills;
    public List<Passive> Passives;
    
    public abstract EquippableSlot Slot { get; }

    //public 
    
    #region Declarations

    [Serializable]
        public enum EquippableSlot
        {
            Accessory,
            Body,
            Feet,
            Hand,
            Head,
            Weapon
        }
    
        [Serializable]
        public enum ArmorType
        {
            Light,
            Heavy
        }
        
        [Serializable]
        public enum EquippableTier
        {
            Damaged,
            Normal,
            Rare,
            Epic,
            Masterpiece
        }

    #endregion
}

public class EquippableTierRewards
{
    public EquippableBase.EquippableTier EquippableTier;
}