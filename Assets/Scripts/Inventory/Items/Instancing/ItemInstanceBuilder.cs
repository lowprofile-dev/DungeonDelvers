using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using SkredUtils;
using UnityEngine;

public static class ItemInstanceBuilder
{
    public static Item BuildInstance(int uid, bool forceDefault = false)
    {
        var database = GameSettings.Instance.ItemDatabase;

        if (database == null)
        {
            Debug.LogError("Item Database Missing");
            Application.Quit();
        }

        var validUid = database.Content.TryGetValue(uid, out var itemBase);

        if (!validUid)
        {
            Debug.LogError($"UID inválido: {uid}");
        }

        return BuildInstance(itemBase,forceDefault);
    }

    public static Item BuildInstance(ItemBase baseItem, bool forceDefault = false)
    {
        if (baseItem is EquippableBase equippableBase)
        {
            var equippable = new Equippable(equippableBase);

            if (forceDefault) return equippable;
            
            equippable.Tier = equippableBase.GetRandomTier();
            return equippable;
        }
        if (baseItem is ConsumableBase consumableBase)
            return new Consumable(consumableBase);
        if (baseItem is MiscItemBase miscItemBase)
            return new MiscItem(miscItemBase);
        return null;
    }

    public static Item BuildInstance(ItemSave itemSave)
    {
        if (itemSave.baseUid == -1)
            return null;
        
        if (itemSave is EquippableSave equippableSave)
            return new Equippable(equippableSave);
        if (itemSave is ConsumableSave consumableSave)
            return new Consumable(consumableSave);
        if (itemSave is MiscItemSave miscItemSave)
            return new MiscItem(miscItemSave);
        return null;
    }
}
