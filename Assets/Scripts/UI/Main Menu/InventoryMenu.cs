using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

public class InventoryMenu : MonoBehaviour
{
    public MainMenu MainMenu = null;
    public GameObject ItemButtonPrefab;
    public RectTransform ScrollContent;
    public InventoryItemInspector InventoryItemInspector;
    [ReadOnly] public List<GameObject> ItemButtons = new List<GameObject>();

    public void OpenInventory()
    {
        gameObject.SetActive(true);
        SetupItems(PlayerController.Instance.Inventory);
        InventoryItemInspector.Inspect(null);
    }
    
    public void SetupItems(IEnumerable<Item> items)
    {
        ItemButtons.ForEach(itemButton => Destroy(itemButton));
        ItemButtons = new List<GameObject>();

        foreach (var item in items)
        {
            var itemObject = Instantiate(ItemButtonPrefab, ScrollContent);
            var inventoryButton = itemObject.GetComponent<InventoryItemButton>();
            inventoryButton.Setup(this, item);
            itemObject.SetActive(true);
            ItemButtons.Add(itemObject);
        }
    }
    
    public void ExitInventory()
    {
        gameObject.SetActive(false);
        MainMenu.OpenMainMenu();
    }
    
    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            ExitInventory();
        }
    }

    public void FilterItems(Predicate<Item> predicate)
    {
        var filteredItems = PlayerController.Instance.Inventory.Where(item => predicate(item));
        SetupItems(filteredItems);
    }

    public void FilterAll() => SetupItems(PlayerController.Instance.Inventory);
    public void FilterEquipment() => FilterItems(item => item is Equippable);
    public void FilterConsumable() => FilterItems(item => item is Consumable);
    public void FilterMisc() => FilterItems(item => item is MiscItem);
    
    public void Inspect(Item item)
    {
        InventoryItemInspector.Inspect(item);
    }
}
