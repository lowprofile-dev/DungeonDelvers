using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour
{
    public ShopMenu ShopMenu;
    public TMP_Text ItemName;
    public TMP_Text ItemPrice;
    public Image ItemImage;
    public ShopItem ShopItem = null;
    
    public void BuildButton(ShopItem item, ShopMenu shopMenu)
    {
        ShopMenu = shopMenu;
        ShopItem = item;
        ItemName.text = item.Item.itemName;
        ItemImage.sprite = item.Item.itemIcon;
        ItemPrice.text = $"{item.Price}g";
    }

    public void SelectItem()
    {
        ShopMenu.Inspect(ShopItem);
    }
}
