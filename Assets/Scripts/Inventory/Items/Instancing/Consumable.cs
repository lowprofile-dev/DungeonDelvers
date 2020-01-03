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

    //Lembrar de fazer função de quando recebe varios consumiveis de uma vez, funções de stackar, funções de juntar
    //stacks quando uma esvazia.
    
    public override ItemSave Serialize() => throw new NotImplementedException();
    
    public ConsumableBase ConsumableBase => Base as ConsumableBase;

    public Consumable(ConsumableBase consumableBase) : base(consumableBase)
    {
        
    }

    public Consumable(ConsumableSave consumableSave) : base(consumableSave)
    {
        
    }
    
}