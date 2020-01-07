using UnityEngine;

[CreateAssetMenu(menuName = "Items/Equippable/HeadBase")]
public class HeadBase : EquippableBase, IArmorTypeEquipment
{
    [SerializeField] private ArmorType _armorType;
    public ArmorType ArmorType => _armorType;
    public override EquippableSlot Slot => EquippableSlot.Head;
}