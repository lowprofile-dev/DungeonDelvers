using System;
using UnityEngine;

public class MiscItem : Item, IStackable
{
    public override ItemSave Serialize() => throw new NotImplementedException();

    public IStackableBase StackableBase => Base as IStackableBase;
    public MiscItemBase MiscItemBase => Base as MiscItemBase;
    
    public MiscItem(MiscItemBase miscItemBase) : base(miscItemBase)
    {
        
    }

    public MiscItem(MiscItemSave miscItemSave) : base(miscItemSave)
    {
        
    }

    public int MaxStack => MiscItemBase.MaxStack;
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
}