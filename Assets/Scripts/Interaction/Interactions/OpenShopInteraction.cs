using System.Collections;
using UnityEngine;

public class OpenShopInteraction : Interaction
{
    public Shop shop;
    public GameObject ShopMenu;
    private ShopMenu menu;
    
    public override void Run(Interactable source)
    {
        var shopObject = Object.Instantiate(ShopMenu);
        var shopMenu = shopObject.GetComponent<ShopMenu>();
        shopMenu.OpenShopMenu(shop);
        menu = shopMenu;
    }

    public override IEnumerator Completion => new WaitWhile(() => menu != null);
}