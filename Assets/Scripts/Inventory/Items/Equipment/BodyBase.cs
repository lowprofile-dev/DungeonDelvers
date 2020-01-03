using UnityEngine;

[CreateAssetMenu(menuName = "Items/Equippable/BodyBase")]
public class BodyBase : EquippableBase
{
    public enum BodyType
    {
        //Melhorar dps(?)
        Light,
        Medium,
        Heavy
    }

    public BodyType bodyType;
    
    public override EquippableSlot Slot => EquippableSlot.Body;
}