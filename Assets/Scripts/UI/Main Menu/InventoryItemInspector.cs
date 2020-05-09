using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SkredUtils;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InventoryItemInspector : MonoBehaviour
{
    public InventoryMenu InventoryMenu;
    public TMP_Text ItemName;
    public TMP_Text EquipStatsText;
    public GameObject Separator;
    public Image ItemImage;
    public TMP_Text ItemDescription;

    public InventoryItemButton SelectedItem;

    public Button UseButton;
    public Button DropButton;

    public void Inspect(InventoryItemButton inventoryItemButton)
    {
        if (inventoryItemButton == null)
        {
            SelectedItem = null;
            ItemName.text = "";
            ItemImage.enabled = false;
            ItemDescription.text = "";
            UseButton.interactable = false;
            DropButton.interactable = false;
            return;
        }

        SelectedItem = inventoryItemButton;
        var item = inventoryItemButton.Item;
        ItemName.text = item.ColoredInspectorName;
        ItemImage.enabled = true;
        ItemImage.sprite = item.Base.itemIcon;
        ItemDescription.text = item.InspectorDescription;

        if (item is Equippable equippable)
        {
            EquipStatsText.gameObject.SetActive(true);
            Separator.SetActive(true);
            EquipStatsText.text = equippable.StatsDescription;
        }
        else
        {
            EquipStatsText.gameObject.SetActive(false);
            Separator.SetActive(false);
        }
        
        UseButton.interactable = item is Consumable consumable && consumable.ConsumableBase.ConsumableUses.Any() 
                                 /*|| item is Equippable*/;
        
        DropButton.interactable = item.Base.droppable;
    }

    public void UseItem()
    {
        //Dar throw error se não existir nenhum item no inventário do selected
        if (SelectedItem.Item is Equippable)
        {
            throw new NotImplementedException();
        }
        else if (SelectedItem.Item is Consumable consumable)
        {
            StartCoroutine(UseItemCoroutine(consumable));
        }
    }


    private IEnumerator UseItemCoroutine(Consumable consumable)
    {
        Character target = null;

        if (consumable.Quantity <= 0)
        {
            PlayerController.Instance.ValidateInventory();
            yield break;
        }

        if (consumable.ConsumableBase.RequiresTarget)
        {
            InventoryMenu.MainMenu.CharacterSelector.StartSelection();
            void SelectTarget(Character character) => target = character;
            InventoryMenu.MainMenu.CharacterSelector.CharactedSelected.AddListener(SelectTarget);
            while (target == null)
                yield return null;

            yield return consumable.ConsumableBase.TargetedUseCoroutine(target);
        }
        else
        {
            yield return consumable.ConsumableBase.UseCoroutine();
        }

        consumable.Quantity--;
        if (consumable.Quantity == 0)
        {
            Destroy(SelectedItem.gameObject);
            PlayerController.Instance.RemoveItemFromInventory(consumable);
            Inspect(null);
        }
        else
        {
            SelectedItem.Setup(InventoryMenu, SelectedItem.Item);
        }
    }

    public void DropItem()
    {
        if (SelectedItem.Item is IStackable stackable)
        {
            stackable.Quantity--;
            if (stackable.Quantity == 0)
            {
                PlayerController.Instance.Inventory.Remove(SelectedItem.Item);
                var itemBase = SelectedItem.Item.Base;
                
                Destroy(SelectedItem.gameObject);
                
                if (PlayerController.Instance.GetQuantityOfItem(itemBase) != 0)
                {
                    var otherInstance =
                        InventoryMenu.ItemButtons.FindLast(itemButton =>
                            itemButton.GetComponent<InventoryItemButton>().Item.Base == itemBase);

                    Inspect(otherInstance.GetComponent<InventoryItemButton>());
                }
                else
                {
                    Inspect(null);
                }
            }
            else
            {
                SelectedItem.Setup(InventoryMenu, SelectedItem.Item);
            }
        }
        else
        {
            PlayerController.Instance.RemoveItemFromInventory(SelectedItem.Item);
            Destroy(SelectedItem.gameObject);
            Inspect(null);
        }
    }
}