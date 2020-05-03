using System;
using System.Collections;
using UnityEngine;

[InteractableNode(defaultNodeName = "Shop")]
public class ShopInteraction : Interaction
{
    [Input] public Shop Shop;
    [Input] public GameObject ShopMenu;

    private void Reset()
    {
        ShopMenu = GameSettings.Instance.DefaultShopMenu;
    }

    public override IEnumerator PerformInteraction(Interactable source)
    {
        var shop = GetInputValue("Shop", Shop);
        var shopMenuPrefab = GetInputValue("ShopMenu", ShopMenu);

        var shopObject = Instantiate(shopMenuPrefab);
        var shopMenu = shopObject.GetComponent<ShopMenu>();
        shopMenu.OpenShopMenu(shop);
        
        yield return new WaitUntil(() => shopObject == null);
    }
}
