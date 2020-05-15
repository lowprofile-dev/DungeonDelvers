using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class DispelStatusEffectTypeEffect : Effect
{
    public StatusEffect.StatusEffectType DispelType;
    
    public override EffectResult ExecuteEffect(SkillInfo skillInfo)
    {
        var target = skillInfo.Target;

        var targetStatusEffectsOfType =
            target.StatusEffectInstances.Where(instance => instance.StatusEffect.Type == DispelType).ToArray();

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        var statusEffectNames = targetStatusEffectsOfType.Select(sE => sE.StatusEffect.StatusEffectName);
        var message = string.Join(", ", statusEffectNames);
        Debug.Log($"Dispelled status effects: {message}");
#endif

        foreach (var instance in targetStatusEffectsOfType)
        {
            target.RemoveStatusEffectAsync(instance);
        }

        return new DispelStatusEffectTypeEffectResult
        {
            skillInfo = skillInfo,
            StatusEffectInstancesDispelled = targetStatusEffectsOfType
        };
    }

    public override object Clone()
    {
        return new DispelStatusEffectTypeEffect
        {
            ElementOverride = ElementOverride,
            DispelType = DispelType
        };
    }

    public class DispelStatusEffectTypeEffectResult : EffectResult
    {
        public StatusEffectInstance[] StatusEffectInstancesDispelled;
    }
}