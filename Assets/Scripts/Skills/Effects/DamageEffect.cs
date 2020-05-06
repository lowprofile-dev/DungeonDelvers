using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Sirenix.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

public class DamageEffect : Effect
{
    public float DamageFactor = 1.0f;
    public DamageType DamageType = DamageType.Physical;

    public override EffectResult ExecuteEffect(SkillInfo skillInfo)
    {
        var damageCalculationInfo = new DamageCalculationInfo
        {
            DamageElement = skillInfo.Skill != null? skillInfo.Skill.Element : Element.None,
            DamageType = DamageType,
            Source = skillInfo.Source,
            Target = skillInfo.Target
        };

        if (ElementOverride.HasValue)
            damageCalculationInfo.DamageElement = ElementOverride.Value;

        var passiveEffects = skillInfo.Target.PassiveEffects.ToArray();

        var overrideDamageEffects = passiveEffects
            .Where(pE => pE is IOverrideDamagePassiveEffect)
            .Cast<IOverrideDamagePassiveEffect>()
            .ToArray();

        foreach (var overrideDamageEffect in overrideDamageEffects)
        {
            var overrideResult = overrideDamageEffect.Override(skillInfo, this);
            if (overrideResult != null)
                return overrideResult;
        }

        var damageCalculationPassives = passiveEffects
            .Where(passiveEffect => passiveEffect is IDamageCalculationInfoOverride)
            .Cast<IDamageCalculationInfoOverride>()
            .ToArray();

        damageCalculationPassives
            .ForEach(damageCalculationPassive =>
                damageCalculationPassive.OverrideDamageCalculationInfo(ref damageCalculationInfo));
        
        var damage = (int)(Mathf.Max(0,BattleController.Instance.DamageCalculation(damageCalculationInfo)) * DamageFactor);

        Debug.Log($"Calculado {damage}");
        
        var targetPassives = skillInfo.Target.PassiveEffects
            //.SelectMany(passive => passive.Effects.Where(passiveEffect => passiveEffect is IReceiveDamagePassiveEffect))
            .Where(pE => pE is IReceiveDamagePassiveEffect)
            .OrderByDescending(effect => effect.Priority)
            .Cast<IReceiveDamagePassiveEffect>()
            .ToArray();

        targetPassives.ForEach(targetPassive => targetPassive.BeforeReceive(skillInfo, ref damage));
        
        Debug.Log($"{skillInfo.Source} causou {damage} de dano em {skillInfo.Target}. Elemento: {damageCalculationInfo.DamageElement}");

        skillInfo.Target.CurrentHp -= damage;
        return new DamageEffectResult
        {
            DamageDealt = damage,
            skillInfo = skillInfo
        };
    }

    public class DamageEffectResult : EffectResult
    {
        public int DamageDealt;
    }

    public struct DamageCalculationInfo
    {
        public Battler Source;
        public Battler Target;
        public DamageType DamageType;
        public Element DamageElement;
        
        //mais info que tiver;
    }
    
    public interface IDealDamagePassiveEffect
    {
        void BeforeDeal(SkillInfo skillInfo, ref int finalDamage);
    }
    
    public interface IOverrideDamagePassiveEffect
    {
        [CanBeNull] EffectResult Override(SkillInfo skillInfo, DamageEffect damageEffect);
    }

    public interface IDamageCalculationInfoOverride
    {
        void OverrideDamageCalculationInfo(ref DamageCalculationInfo damageCalculationInfo);
    }
    
    public interface IReceiveDamagePassiveEffect
    {
        void BeforeReceive(SkillInfo skillInfo, ref int finalDamage);
    }
}














