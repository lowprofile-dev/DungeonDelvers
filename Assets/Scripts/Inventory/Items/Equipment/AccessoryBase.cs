using UnityEngine;

[CreateAssetMenu(menuName = "Items/Equippable/AccessoryBase")]
public class AccessoryBase : EquippableBase
{
    public override EquippableSlot Slot => EquippableSlot.Accessory;
}