using UnityEngine;

[CreateAssetMenu(menuName = "Items/Equippable/BodyBase")]
public class BodyBase : EquippableBase
{
    public ArmorType ArmorType;
    public override EquippableSlot Slot => EquippableSlot.Body;
}