using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;

public class Mastery
{
    public string MasteryName;
    [ValidateInput("_validatePositive", "Max Level precisa ser maior ou igual a 1")] public int MasteryMaxLevel = 1;
    [ValidateInput("_validatePositive", "MP Cost precisa ser maior ou igual a 1")] public int MPCost = 1;
    public List<MasteryCondition> Conditions = new List<MasteryCondition>();
    public List<MasteryEffect> Effects = new List<MasteryEffect>();
    
#if UNITY_EDITOR
    private bool _validatePositive(int value) => value >= 1;

    private string _masteryElementName => !string.IsNullOrWhiteSpace(MasteryName) ? MasteryName : "Mastery";
#endif
}