public class ApplyStatusEffectEffect : Effect
{
    public StatusEffect StatusEffect = new StatusEffect();
    
    public override EffectResult ExecuteEffect(SkillInfo skillInfo)
    {
        StatusEffect.Apply(skillInfo);
        
        return new ApplyStatusEffectResult
        {
            Applied = StatusEffect,
            skillInfo = skillInfo
        };
    }
    
    public class ApplyStatusEffectResult : EffectResult
    {
        public StatusEffect Applied;
    }
}
