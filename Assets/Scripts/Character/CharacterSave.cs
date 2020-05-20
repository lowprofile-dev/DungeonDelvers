using System;
using System.Collections.Generic;


[Serializable] public struct CharacterSave
{
    public int baseUid;
    public int currentHp;
    //public SerializedMasteryInstance[] serializedMasteryInstances;
    public SerializedTechInstance[] serializedTechInstances;
    //public MasteryInstance[] MasteryInstances;
    public EquippableSave[] Equipment;
    public int masteryPoints;
}