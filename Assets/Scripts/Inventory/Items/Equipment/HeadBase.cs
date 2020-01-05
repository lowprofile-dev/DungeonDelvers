using UnityEngine;

[CreateAssetMenu(menuName = "Items/Equippable/HeadBase")]
public class HeadBase : EquippableBase
{
    public ArmorType ArmorType;
    public override EquippableSlot Slot => EquippableSlot.Head;
}