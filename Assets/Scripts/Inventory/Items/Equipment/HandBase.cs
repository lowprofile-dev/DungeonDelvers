using UnityEngine;

[CreateAssetMenu(menuName = "Items/Equippable/HandBase")]
public class HandBase : EquippableBase, IArmorTypeEquipment
{
    [SerializeField] private ArmorType _armorType;
    public ArmorType ArmorType => _armorType;
    public override EquippableSlot Slot => EquippableSlot.Hand;
}