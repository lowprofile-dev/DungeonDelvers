using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities;
using UnityEngine;
#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
#endif


public abstract class ItemBase : SerializableAsset
{
    [AssetIcon] public Sprite itemIcon;
    public string itemName = "";
    public int goldValue = 0;
    public bool sellable => goldValue > 0;
    public bool droppable = true;
    [TextArea] public string itemText = "";
}

public interface IStackableBase
{
    ItemBase ItemBase { get; }
    int MaxStack { get; }
}

public interface IStackable
{
    IStackableBase StackableBase { get; }
    Item Item { get; }
    int MaxStack { get; }
    int Quantity { get; set; }
}
