using System;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item
{
    public abstract ItemSave Serialize();

    public Item Copy()
    {
        return ItemInstanceBuilder.BuildInstance(Serialize());
    }

    [ShowInInspector] public ItemBase Base { get; private set; }

    public Item(ItemBase itemBase)
    {
        Base = itemBase;
    }

    public Item(ItemSave itemSave)
    {
        //Base = ItemDatabase.Instance.Items.Find(x => x.uniqueIdentifier == itemSave.baseUid);
        try
        {
            Base = GameSettings.Instance.ItemDatabase.Content[itemSave.baseUid];
        }
        catch (Exception e)
        {
            Debug.LogError($"Falha em deserializar id {itemSave.baseUid}");
            throw new DeserializationFailureException(typeof(ItemSave));
        }
    }

    public virtual string InspectorName => Base.itemName;
    public virtual string InspectorDescription => Base.itemText;
    public virtual string ColoredInspectorName => InspectorName;
}

//Ver como vai ser a serialização dos itens levando em conta os tipos diferentes