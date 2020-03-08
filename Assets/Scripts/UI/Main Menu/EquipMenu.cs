using System.Collections.Generic;
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
    [ReadOnly] public List<GameObject> Buttons = new List<GameObject>();
    
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
            }).ToArray();

        var equippableItemsInOtherCharacters = PlayerController.Instance.Party.Where(partyMember => partyMember != character).Select(partyMember =>
        {
            var slottedEquip = partyMember.GetSlot(slot);
            if (slottedEquip == null)
                return (null, null);
            
            if (slottedEquip.Base is WeaponBase weapon &&
                character.Base.WeaponTypes.Contains(weapon.weaponType))
                return (slottedEquip, partyMember);
            else if (slottedEquip.Base is IArmorTypeEquipment armor &&
                     character.Base.ArmorTypes.Contains(armor.ArmorType))
                return (slottedEquip, partyMember);
            else
                return (null, null);
        }).Where(s => s.slottedEquip != null).ToArray();

        var equips = "";

        equippableItemsInInventory.ForEach(e => equips += $"{e.EquippableBase.itemName} -- (Inventory) // ");
        equippableItemsInOtherCharacters.ForEach(s =>
            equips += $"{s.slottedEquip.EquippableBase.itemName} -- ({s.partyMember.Base.CharacterName}) // ");

        Debug.Log(equips);
        
        //Cleanup Children
        foreach (var button in Buttons)
        {
            Destroy(button);
        }

        equippableItemsInInventory.ForEach(SetUpEquipButton);
        equippableItemsInOtherCharacters.ForEach(SetUpEquipButton);
        
        // equippableItemsInOtherCharacters.ForEach(item =>
        // {
        //     var button = Instantiate(EquipButtonPrefab, EquipmentGrid);
        //     var equipButton = button.GetComponent<EquipButton>();
        //     equipButton.Image.sprite = item.slottedEquip.Base.itemIcon;
        //     equipButton.Button.onClick.AddListener(() =>
        //     {
        //         var oldEquip = Character.Unequip(slot);
        //         var newEquip = item.partyMember.Unequip(slot);
        //         
        //         Character.Equip(newEquip);
        //
        //         //Refazer isso menos bugado.
        //         if (oldEquip.Base is WeaponBase weapon)
        //         {
        //             if (item.partyMember.Base.WeaponTypes.Contains(weapon.weaponType))
        //                 item.partyMember.Equip(oldEquip);
        //             else
        //             {
        //                 var possibleEquips = PlayerController.Instance.Inventory.Where(i => i is Equippable)
        //                     .Cast<Equippable>()
        //                     .Where(e => e.Base is WeaponBase)
        //                     .Where(w => item.partyMember.Base.WeaponTypes.Contains((w.Base as WeaponBase).weaponType));
        //
        //                 var rand = Random.Range(0, possibleEquips.Count());
        //                 
        //                 item.partyMember.Equip(possibleEquips.ElementAt(rand));
        //             }
        //         } else if (oldEquip.Base is IArmorTypeEquipment equipment)
        //         {
        //             if (item.partyMember.Base.ArmorTypes.Contains(equipment.ArmorType))
        //                 item.partyMember.Equip(oldEquip);
        //         }
        //         CloseEquipMenu();
        //     });
        //     equipButton.Text.text = $"{item.slottedEquip.InspectorName}\n<#c0c0c0ff><size=22>(Equipped by {item.partyMember.Base.CharacterName})</size></color>";
        //     Buttons.Add(button);
        // });
    }

    public void Update()
    {
        if (Input.GetButtonDown("Cancel"))
            CloseEquipMenu();
    }

    private void SetUpEquipButton(Equippable item)
    {
        var button = Instantiate(EquipButtonPrefab, EquipmentGrid);
        var equipButton = button.GetComponent<EquipButton>();
        equipButton.Image.sprite = item.Base.itemIcon;
        equipButton.Button.onClick.AddListener(() =>
        {
            Character.Equip(item);
            CloseEquipMenu();
        });
        equipButton.Text.text = item.InspectorName;
        Buttons.Add(button);
    }

    private void SetUpEquipButton((Equippable equip, Character equippedTo) equippedItem)
    {
        var otherCharacter = equippedItem.equippedTo;
        var item = equippedItem.equip;
        
        var button = Instantiate(EquipButtonPrefab, EquipmentGrid);
        var equipButton = button.GetComponent<EquipButton>();
        equipButton.Image.sprite = equippedItem.equip.Base.itemIcon;
        equipButton.Button.onClick.AddListener(() =>
        {
            var oldEquip = Character.Unequip(equippedItem.equip.Slot);
            var newEquip = equippedItem.equippedTo.Unequip(equippedItem.equip.Slot);
            
            Character.Equip(newEquip);

            if (equippedItem.equippedTo.CanEquip(oldEquip))
            {
                equippedItem.equippedTo.Equip(oldEquip);
            }
            CloseEquipMenu();
        });
        equipButton.Text.text = $"{item.InspectorName}\n<#c0c0c0ff><size=22>(Equipped by {otherCharacter.Base.CharacterName})</size></color>";
        Buttons.Add(button);
    }

    public void CloseEquipMenu()
    {
        gameObject.SetActive(false);
        CharacterInspector.OpenCharacterInspector(Character);
    }
}