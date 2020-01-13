using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Equippable/WeaponBase")]
public class WeaponBase : EquippableBase
{
    [Serializable]
    public enum WeaponType
    {
        Sword1H,
        Sword2H,
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