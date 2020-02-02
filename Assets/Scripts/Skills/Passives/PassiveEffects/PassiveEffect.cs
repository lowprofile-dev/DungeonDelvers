using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using Unity.Collections;
using UnityEngine;

public abstract class PassiveEffect
{
    [Sirenix.OdinInspector.ReadOnly] public IPassiveEffectSource PassiveSource;
    public int Priority = 0;
    public abstract PassiveEffect GetInstance();
}

