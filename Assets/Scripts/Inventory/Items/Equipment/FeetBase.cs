using UnityEngine;

[CreateAssetMenu(menuName = "Items/Equippable/FeetBase")]
public class FeetBase : EquippableBase
{
    public override EquippableSlot Slot => EquippableSlot.Feet;
}