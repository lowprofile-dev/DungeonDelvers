using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopInspector : MonoBehaviour
{
    public ShopMenu ShopMenu;
    public ShopItem Item;

    public TMP_Text ItemName;
    public Image ItemImage;
    public TMP_Text ItemPrice;
    public TMP_Text ItemDescription;
    public Button BuyButton;
    
    public void InspectItem(ShopItem shopItem)
    {
        var item = shopItem.Item;
        Item = shopItem;
        ItemName.text = item.name;
        ItemDescription.text = item.itemText;
        ItemPrice.text = $"{shopItem.Price}g";

        if (shopItem.Price > PlayerController.Instance.CurrentGold)
        {
            BuyButton.interactable = false;
        }
    }

    public void BuyItem()
    {
        var player = PlayerController.Instance;
        if (player.CurrentGold >= Item.Price)
        {
            player.CurrentExp -= Item.Price;
            player.AddItemToInventory(Item.Item);
            ShopMenu.UpdateGoldText();
        }
        else
            Debug.LogError("Insufficient Gold");
    }
}
