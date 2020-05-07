using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopMenu : MonoBehaviour
{
    public Shop Shop;
    public ShopInspector Inspector;
    public TMP_Text GoldText;
    public RectTransform ItemLayout;
    public GameObject ItemPrefab;
    
    private void Start()
    {
        UpdateGoldText();
    }

    public void OpenShopMenu(Shop shop)
    {
        Shop = shop;
        BuildItems(shop.Items);
        Inspect(null);
    }

    public void Inspect(ShopItem item)
    {
        Inspector.InspectItem(item);
    }

    private void BuildItems(IEnumerable<ShopItem> shopItems)
    {
        foreach (var shopItem in shopItems)
        {
            var itemObject = Instantiate(ItemPrefab, ItemLayout);
            var shopButton = itemObject.GetComponent<ShopButton>();
            shopButton.BuildButton(shopItem,this);
        }
    }

    public void UpdateGoldText()
    {
        GoldText.text = $"{PlayerController.Instance.CurrentGold}g";
    }
    
    public void CloseMenu()
    {
        Destroy(gameObject);
    }
}
