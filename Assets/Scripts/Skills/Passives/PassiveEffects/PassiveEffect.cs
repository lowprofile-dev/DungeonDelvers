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
    public int Priority = 0;
}

public struct PassiveEffectInfo
{
    public string PassiveEffectSourceName;
    public Battler Source;
    public Battler Target;
}
