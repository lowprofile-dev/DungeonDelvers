﻿using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;


[Obsolete] public class _Mastery
{
    public string MasteryName;

    [ValidateInput("_validatePositive", "Max Level precisa ser maior ou igual a 1")]
    public int MasteryMaxLevel = 1;

    [ValidateInput("_validatePositive", "MP Cost precisa ser maior ou igual a 1")]
    public int MPCost = 1;

    public List<MasteryCondition> Conditions = new List<MasteryCondition>();
    public List<_MasteryEffect> Effects = new List<_MasteryEffect>();

#if UNITY_EDITOR
    private bool _validatePositive(int value) => value >= 1;

    private string _masteryElementName => !string.IsNullOrWhiteSpace(MasteryName) ? MasteryName : "Mastery";
#endif
}