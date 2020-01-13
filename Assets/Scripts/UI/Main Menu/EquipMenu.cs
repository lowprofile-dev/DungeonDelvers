using Sirenix.OdinInspector;
using System.Linq;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class EquipMenu : MonoBehaviour
{
    public CharacterInspector CharacterInspector;
    [ReadOnly] public Character Character;
    public RectTransform EquipmentGrid;
    public GameObject EquipButtonPrefab;

    public void BuildEquips(Character character, EquippableBase.EquippableSlot slot)
    {
        Character = character;
        gameObject.SetActive(true);

        var equippableItemsInInventory = PlayerController.Instance.Inventory.Where(item => item is Equippable)
            .Cast<Equippable>()
            .Where(equippable => equippable.EquippableBase.Slot == slot)
            .Where(equippable =>
            {
                if (equippable.Base is WeaponBase weapon)
                    return character.Base.WeaponTypes.Contains(weapon.weaponType);
                else if (equippable.Base is IArmorTypeEquipment armor)
                    return character.Base.ArmorTypes.Contains(armor.ArmorType);
                else
                    return false;
            });

        var equippableItemsInOtherCharacters = PlayerController.Instance.Party.Select(partyMember =>
        {
            var slottedEquip = partyMember.GetSlot(slot);

            if (slottedEquip.Base is WeaponBase weapon &&
                character.Base.WeaponTypes.Contains(weapon.weaponType))
                return (slottedEquip, partyMember);
            else if (slottedEquip.Base is IArmorTypeEquipment armor &&
                     character.Base.ArmorTypes.Contains(armor.ArmorType))
                return (slottedEquip, partyMember);
            else
                return (null, null);
        }).Where(s => s.slottedEquip != null);

        var equips = "";

        equippableItemsInInventory.ForEach(e => equips += $"{e.EquippableBase.itemName} -- (Inventory) // ");
        equippableItemsInOtherCharacters.ForEach(s =>
            equips += $"{s.slottedEquip.EquippableBase.itemName} -- ({s.partyMember.Base.CharacterName}) // ");

        Debug.Log(equips);
    }

    public void Update()
    {
        if (Input.GetButtonDown("Cancel"))
            CloseEquipMenu();
    }

    public void CloseEquipMenu()
    {
        gameObject.SetActive(false);
        CharacterInspector.OpenCharacterInspector(Character);
    }
}