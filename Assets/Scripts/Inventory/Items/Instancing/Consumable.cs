using System;
using UnityEngine;

public class Consumable : Item, IStackable
{
    public int MaxStack => ConsumableBase.MaxStack;
    [SerializeField] private int quantity;

    public int Quantity
    {
        get => quantity;
        set
        {
            quantity = value;
            Mathf.Clamp(quantity, 0, MaxStack);
        }
    }

    public Item Item => this;
    
    public override ItemSave Serialize()
    {
        return new ConsumableSave
        {
            baseUid = GameSettings.Instance.ItemDatabase.GetId(Base).Value,
            Quantity = quantity
        };
    }
    
    public IStackableBase StackableBase => Base as IStackableBase;

    public ConsumableBase ConsumableBase => Base as ConsumableBase;

    public Consumable(ConsumableBase consumableBase) : base(consumableBase)
    {
        Quantity = 1;
    }

    public Consumable(ConsumableSave consumableSave) : base(consumableSave)
    {
        Quantity = consumableSave.Quantity;
    }
    
}