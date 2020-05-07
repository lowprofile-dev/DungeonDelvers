using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Items/Equippable/BodyBase")]
public class BodyBase : EquippableBase
{
    // [SerializeField] private ArmorType _armorType;
    // public ArmorType ArmorType => _armorType;
    [FormerlySerializedAs("_armorType")] public ArmorType ArmorType;
    public override EquippableSlot Slot => EquippableSlot.Body;
}