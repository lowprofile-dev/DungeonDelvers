using UnityEngine;

[CreateAssetMenu(menuName = "Items/Equippable/BodyBase")]
public class BodyBase : EquippableBase, IArmorTypeEquipment
{
    [SerializeField] private ArmorType _armorType;
    public ArmorType ArmorType => _armorType;
    public override EquippableSlot Slot => EquippableSlot.Body;
}