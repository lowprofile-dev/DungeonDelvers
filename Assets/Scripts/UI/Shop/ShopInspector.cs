using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopInspector : MonoBehaviour
{
    public ShopMenu ShopMenu;
    public ShopItem Item = null;

    public TMP_Text ItemName;
    public Image ItemImage;
    public TMP_Text ItemPrice;
    public TMP_Text ItemDescription;
    public Button BuyButton;
    
    public void InspectItem(ShopItem shopItem)
    {
        if (shopItem == null)
        {
            Item = null;
            ItemName.text = "";
            ItemDescription.text = "";
            ItemPrice.text = "";
            ItemImage.enabled = false;
            BuyButton.interactable = false;
            return;
        }
        
        var item = shopItem.Item;
        Item = shopItem;
        ItemImage.enabled = true;
        ItemImage.sprite = item.itemIcon;
        ItemName.text = item.name;
        ItemDescription.text = item.itemText;
        ItemPrice.text = $"{shopItem.Price}g";

        if (shopItem.Price > PlayerController.Instance.CurrentGold)
        {
            BuyButton.interactable = false;
        }
        else
            BuyButton.interactable = true;
    }

    public void BuyItem()
    {
        if (Item == null)
            return;

        var item = Item;
        var player = PlayerController.Instance;

        if (player.CurrentGold >= item.Price)
        {
            player.CurrentGold -= item.Price;
            var builtItem = ItemInstanceBuilder.BuildInstance(item.Item, ShopMenu.Shop.forceDefaults);
            player.AddItemToInventory(builtItem);
            ShopMenu.UpdateGoldText();
        }
        else
            Debug.LogError("Insufficient Gold");
    }
}
