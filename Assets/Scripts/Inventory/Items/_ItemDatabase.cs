using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using SkredUtils;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Items/ItemDatabase")]
public class _ItemDatabase : SerializedScriptableObject
{
    public string DatabaseVersion = "";
    public List<ItemBase> Items = new List<ItemBase>();

    public static _ItemDatabase Instance
    {
        get
        {
            var resource = Resources.Load("Items/ItemDatabase");
            
            if (resource is _ItemDatabase database)
                return database;
            return null;
        }
    }
    
#if UNITY_EDITOR
    // [Button(Name = "Load All")]
    // void LoadAllItems()
    // {
    //     var resources = Resources.LoadAll("");
    //     Items = new List<ItemBase>();
    //
    //     foreach (var resource in resources)
    //     {
    //         
    //     }
    // }
#endif
}
