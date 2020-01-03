using UnityEngine;

[CreateAssetMenu(menuName = "Items/Equippable/AccessoryBase")]
public class AccessoryBase : EquippableBase
{
    public enum AccessoryType
    {
        Ring
    }

    public AccessoryType accessoryType;

    public override EquippableSlot Slot => EquippableSlot.Accessory;
}