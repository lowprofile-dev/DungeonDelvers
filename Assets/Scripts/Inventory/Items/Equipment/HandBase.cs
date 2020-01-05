using UnityEngine;

[CreateAssetMenu(menuName = "Items/Equippable/HandBase")]
public class HandBase : EquippableBase
{
    public ArmorType ArmorType;
    public override EquippableSlot Slot => EquippableSlot.Hand;
}