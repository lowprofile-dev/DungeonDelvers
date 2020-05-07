using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Shop : SerializedMonoBehaviour
{
    public bool forceDefaults = true;
    public List<ShopItem> Items;
}

[Serializable]
public class ShopItem
{
    public ItemBase Item;
    public bool IsFixedPrice;
    [ShowIf("IsFixedPrice")] public int FixedPrice;

    public int Price
    {
        get
        {
            if (IsFixedPrice)
                return FixedPrice;
            else
                return (int) (Item.goldValue * GameSettings.Instance.BuyModifier * 2);
        }
    }
}