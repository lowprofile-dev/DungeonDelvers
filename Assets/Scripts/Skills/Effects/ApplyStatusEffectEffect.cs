public class ApplyStatusEffectEffect : Effect
{
    public StatusEffect StatusEffect;
    public int turnDuration;
    
    public override EffectResult ExecuteEffect(SkillInfo skillInfo)
    {
        StatusEffect.Apply(skillInfo, turnDuration);
        
        return new ApplyStatusEffectResult
        {
            Applied = StatusEffect,
            skillInfo = skillInfo
        };
    }

    public override object Clone()
    {
        return new ApplyStatusEffectEffect
        {
            ElementOverride = ElementOverride,
            StatusEffect = StatusEffect,
            turnDuration = turnDuration
        };
    }

    public class ApplyStatusEffectResult : EffectResult
    {
        public StatusEffect Applied;
    }
}
