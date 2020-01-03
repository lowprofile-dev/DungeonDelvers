using UnityEngine;

[CreateAssetMenu(menuName = "Items/Equippable/HandBase")]
public class HandBase : EquippableBase
{
    public enum HandType
    {
        Light,
        Medium,
        Heavy
    }

    public HandType handType;

    public override EquippableSlot Slot => EquippableSlot.Hand;
}