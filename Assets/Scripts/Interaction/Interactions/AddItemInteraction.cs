using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddItemInteraction : Interaction
{
    public ItemBase ItemBase;

    public override void Run(Interactable source)
    {
//        var item = ItemInstanceBuilder.BuildInstance(ItemBase);
//        PlayerController.Instance.Inventory.Add(item);
        PlayerController.Instance.AddItemToInventory(ItemBase);
    }

    public override IEnumerator Completion => null;
}
