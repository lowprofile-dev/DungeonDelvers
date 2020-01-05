using UnityEngine;

[CreateAssetMenu(menuName = "Items/Equippable/BodyBase")]
public class FeetBase : EquippableBase
{
    public ArmorType ArmorType;
    public override EquippableSlot Slot => EquippableSlot.Feet;
}