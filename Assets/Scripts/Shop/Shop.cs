using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Shop : SerializedMonoBehaviour
{
    public List<ShopItem> Items;
    [Range(0, 1)] public float SellModifier;
}

[Serializable]
public class ShopItem
{
    public ItemBase Item;
    public bool IsFixedPrice;
    [ShowIf("IsFixedPrice")] public int FixedPrice;
    [HideIf("IsFixedPrice")] public float PriceMultiplier;

    public int Price
    {
        get
        {
            if (IsFixedPrice)
                return FixedPrice;
            else
                return (int) (Item.goldValue * PriceMultiplier);
        }
    }
}