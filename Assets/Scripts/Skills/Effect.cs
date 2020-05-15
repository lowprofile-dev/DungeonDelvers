using System;
using UnityEngine;

public abstract class Effect : ICloneable
{
    public Element? ElementOverride;
    public abstract EffectResult ExecuteEffect(SkillInfo skillInfo);

    public abstract object Clone();
}