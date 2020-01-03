using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public static class ItemInstanceBuilder
{
    public static Item BuildInstance(string uid)
    {
        var database = ItemDatabase.Instance;

        if (database == null)
            return null;

        var uidQuery = from item in database.Items
            where item.uniqueIdentifier == uid
            select item;

        var itemBases = uidQuery as ItemBase[] ?? uidQuery.ToArray();
        if (!itemBases.Any())
        {
            Debug.LogError($"UID {uid} inválido.");
            return null;
        }

        var baseItem = itemBases.First();

        return BuildInstance(baseItem);
    }

    public static Item BuildInstance(ItemBase baseItem)
    {
        if (baseItem is EquippableBase equippableBase)
            return new Equippable(equippableBase);
        if (baseItem is ConsumableBase consumableBase)
            return new Consumable(consumableBase);
        if (baseItem is MiscItemBase miscItemBase)
            return new MiscItem(miscItemBase);
        return null;
    }

    public static Item BuildInstance(ItemSave itemSave)
    {
        if (itemSave is EquippableSave equippableSave)
            return new Equippable(equippableSave);
        if (itemSave is ConsumableSave consumableSave)
            return new Consumable(consumableSave);
        if (itemSave is MiscItemSave miscItemSave)
            return new MiscItem(miscItemSave);
        return null;
    }
}
