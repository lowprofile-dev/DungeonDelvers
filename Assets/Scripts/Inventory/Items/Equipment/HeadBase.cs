using UnityEngine;

[CreateAssetMenu(menuName = "Items/Equippable/HeadBase")]
public class HeadBase : EquippableBase
{
    public enum HeadType
    {
        Light,
        Medium,
        Heavy
    }

    public HeadType headType;

    public override EquippableSlot Slot => EquippableSlot.Head;
}