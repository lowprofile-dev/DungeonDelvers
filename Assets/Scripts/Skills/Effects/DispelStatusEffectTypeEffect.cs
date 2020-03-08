using System.Linq;

public class DispelStatusEffectTypeEffect : Effect
{
    public StatusEffect.StatusEffectType DispelType;
    
    public override EffectResult ExecuteEffect(SkillInfo skillInfo)
    {
        var target = skillInfo.Target;

        var targetStatusEffectsOfType =
            target.StatusEffectInstances.Where(instance => instance.StatusEffect.Type == DispelType).ToArray();

        foreach (var instance in targetStatusEffectsOfType)
        {
            target.RemoveStatusEffect(instance);
        }

        return new DispelStatusEffectTypeEffectResult
        {
            skillInfo = skillInfo,
            StatusEffectInstancesDispelled = targetStatusEffectsOfType
        };
    }
    
    public class DispelStatusEffectTypeEffectResult : EffectResult
    {
        public StatusEffectInstance[] StatusEffectInstancesDispelled;
    }
}