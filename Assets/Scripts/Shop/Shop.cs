using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Shop : SerializedMonoBehaviour
{
    public bool forceDefaults = true;
    public List<ShopItem> Items;

    private void Awake()
    {
        SortShop();
    }

    private void SortShop()
    {
        Items.Sort((i1, i2) =>
        {
            var price1 = i1.Price;
            var price2 = i2.Price;
            if (price1 != price2) return price1 - price2;
            else return String.Compare(i1.Item.itemName, i2.Item.itemName, StringComparison.Ordinal);
        });
    }
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
                return Mathf.CeilToInt(Item.goldValue * GameSettings.Instance.BuyModifier);
        }
    }
}