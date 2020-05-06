using UnityEngine;

[CreateAssetMenu(menuName = "Items/Equippable/FeetBase")]
public class FeetBase : EquippableBase, IArmorTypeEquipment
{
    [SerializeField] private ArmorType _armorType;
    public ArmorType ArmorType => _armorType;
    public override EquippableSlot Slot => EquippableSlot.Feet;
}