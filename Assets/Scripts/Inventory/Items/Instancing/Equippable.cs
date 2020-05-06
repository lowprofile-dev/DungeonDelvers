using System;
using Sirenix.OdinInspector;

public class Equippable : Item
{
    public override ItemSave Serialize()
    {
        return new EquippableSave
        {
            baseUid = GameSettings.Instance.ItemDatabase.GetId(Base).Value
        };
    }
    
    public EquippableBase EquippableBase => Base as EquippableBase;
    [ShowIf("hasBase")] public EquippableBase.EquippableSlot Slot => EquippableBase.Slot;
    public Equippable(EquippableBase equippableBase) : base(equippableBase)
    {
        //aplicar coisas aqui
    }

    public Equippable(EquippableSave equippableSave) : base(equippableSave)
    {
        
    }
    
#if UNITY_EDITOR
    private bool hasBase() => Base != null;
#endif
}