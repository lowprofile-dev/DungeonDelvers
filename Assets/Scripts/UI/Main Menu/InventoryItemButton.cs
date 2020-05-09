using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemButton : SerializedMonoBehaviour
{
    [ReadOnly] public InventoryMenu InventoryMenu;
    [ReadOnly] public ForgeMenu ForgeMenu;
    [ReadOnly] public Item Item;
    public Image itemImage;
    public TMP_Text itemName;

    public void Setup(InventoryMenu inventoryMenu, Item item)
    {
        InventoryMenu = inventoryMenu;
        Item = item;
        itemImage.sprite = item.Base.itemIcon;
        itemName.text = item.ColoredInspectorName;

        if (item is Consumable consumable)
        {
            itemName.text += $" x{consumable.Quantity}";
        }
    }

    public void Setup(ForgeMenu forgeMenu, Item item, string text = null)
    {
        ForgeMenu = forgeMenu;
        Item = item;
        itemImage.sprite = item.Base.itemIcon;
        if (string.IsNullOrWhiteSpace(text)) itemName.text = item.ColoredInspectorName;
        else itemName.text = text;
    }

    public void OpenItemInfo()
    {
        if (InventoryMenu != null) InventoryMenu.Inspect(this);
        if (ForgeMenu != null) ForgeMenu.Inspect(Item as Equippable);
    }
}
