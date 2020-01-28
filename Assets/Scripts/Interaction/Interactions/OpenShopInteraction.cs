using System.Collections;
using UnityEngine;

public class OpenShopInteraction : Interaction
{
    public Shop shop;
    public GameObject ShopMenu;
    private GameObject menu;
    
    public override void Run(Interactable source)
    {
        var shopObject = Object.Instantiate(ShopMenu);
        var shopMenu = shopObject.GetComponent<ShopMenu>();
        shopMenu.OpenShopMenu(shop);
        menu = shopObject;
    }

    public override IEnumerator Completion => new WaitWhile(() => menu != null);
}