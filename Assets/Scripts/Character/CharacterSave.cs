using System;
using System.Collections.Generic;


[Serializable] public struct CharacterSave
{
    public string baseUid;
    public int currentHp;
    public SerializedMasteryInstance[] serializedMasteryInstances;
    public EquippableSave[] Equipment;
    public int masteryPoints;
}