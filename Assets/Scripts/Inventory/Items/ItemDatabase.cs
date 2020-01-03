using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using SkredUtils;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Items/ItemDatabase")]
public class ItemDatabase : SerializedScriptableObject
{
    public string DatabaseVersion = "";
    public List<ItemBase> Items = new List<ItemBase>();

    public static ItemDatabase Instance
    {
        get
        {
            var resource = Resources.Load("Items/ItemDatabase");
            
            if (resource is ItemDatabase database)
                return database;
            return null;
        }
    }
    
#if UNITY_EDITOR
    [Button(Name = "Load All")]
    void LoadAllItems()
    {
        var resources = Resources.LoadAll("", typeof(ItemBase));
        Items = new List<ItemBase>();

        foreach (var resource in resources)
            Items.Include(resource as ItemBase);
        
    }
#endif
}
