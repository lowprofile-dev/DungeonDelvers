using System;
using Sirenix.OdinInspector;

public class Equippable : Item
{
    public override ItemSave Serialize() => throw new NotImplementedException();
    
    public EquippableBase EquippableBase => Base as EquippableBase;
    [ShowInInspector] public EquippableBase.EquippableSlot Slot => EquippableBase.Slot;
    public Equippable(EquippableBase equippableBase) : base(equippableBase)
    {
        //aplicar coisas aqui
    }

    public Equippable(EquippableSave equippableSave) : base(equippableSave)
    {
        
    }
}