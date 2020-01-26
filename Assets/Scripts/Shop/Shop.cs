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
public struct ShopItem
{
    public ItemBase Item;
    public bool FixedPrice;
    [ShowIf("FixedPrice")] public int Price;
    [HideIf("FixedPrice")] public float PriceMultiplier;
}