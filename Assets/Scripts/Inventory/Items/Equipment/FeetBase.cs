using UnityEngine;

[CreateAssetMenu(menuName = "Items/Equippable/BodyBase")]
public class FeetBase : EquippableBase
{
    public enum FeetType
    {
        //Melhorar dps(?)
        Light,
        Medium,
        Heavy
    }

    public FeetType feetType;

    public override EquippableSlot Slot => EquippableSlot.Feet;
}