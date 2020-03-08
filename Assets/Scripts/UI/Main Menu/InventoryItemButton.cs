using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemButton : SerializedMonoBehaviour
{
    [ReadOnly] public InventoryMenu InventoryMenu;
    [ReadOnly] public Item Item;
    public Image itemImage;
    public Text itemName;

    public void Setup(InventoryMenu inventoryMenu, Item item)
    {
        InventoryMenu = inventoryMenu;
        Item = item;
        itemImage.sprite = item.Base.itemIcon;
        itemName.text = item.Base.itemName;

        if (item is Consumable consumable)
        {
            itemName.text += $" x{consumable.Quantity}";
        }
    }

    public void OpenItemInfo()
    {
        InventoryMenu.Inspect(this);
    }
}
