using UnityEngine;

public abstract class Effect
{
    public Element? ElementOverride;
    public abstract EffectResult ExecuteEffect(SkillInfo skillInfo);
}