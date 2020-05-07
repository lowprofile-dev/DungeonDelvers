using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Equippable/WeaponBase")]
public class WeaponBase : EquippableBase
{
    [Serializable]
    public enum WeaponType
    {
        Shortsword,
        Longsword,
        Axe,
        Knife,
        Staff,
        Tome,
        Spear,
        Bow
    }

    public WeaponType weaponType;

    public override EquippableSlot Slot => EquippableSlot.Weapon;
}