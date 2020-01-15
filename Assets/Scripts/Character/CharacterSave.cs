using System;
using System.Collections.Generic;

[Serializable]
public struct CharacterSave
{
    public string baseUid;
    public int currentHp;
    public int[] masteryLevels;
    public EquippableSave[] Equipment;
    
}