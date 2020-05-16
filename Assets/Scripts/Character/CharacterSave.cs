using System;
using System.Collections.Generic;


[Serializable] public struct CharacterSave
{
    public int baseUid;
    public int currentHp;
    //public SerializedMasteryInstance[] serializedMasteryInstances;
    public MasteriesV3.MasteryInstance[] MasteryInstances;
    public EquippableSave[] Equipment;
    public int masteryPoints;
}