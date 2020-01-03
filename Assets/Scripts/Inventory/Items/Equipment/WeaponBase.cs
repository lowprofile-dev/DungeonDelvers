using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Equippable/WeaponBase")]
public class WeaponBase : EquippableBase
{
    public enum WeaponType
    {
        Sword,
        Knife,
        Staff,
        Axe,
    }

    public WeaponType weaponType;

    public override EquippableSlot Slot => EquippableSlot.Weapon;
}