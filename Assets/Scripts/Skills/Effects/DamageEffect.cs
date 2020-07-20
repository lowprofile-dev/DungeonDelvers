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
        var damageCalculationInfo = BuildDamageCalculationInfo(skillInfo);

        var overrideEffects = new List<PassiveEffect>();
        var passiveEffects = skillInfo.Target.PassiveEffects.ToArray();

        var overrideDamageEffects = passiveEffects
            .Where(pE => pE is IReceiveDamageOverride)
            .Cast<IReceiveDamageOverride>()
            .ToArray();

        foreach (var overrideDamageEffect in overrideDamageEffects)
        {
            var overrideResult = overrideDamageEffect.OverrideReceiveDamage(skillInfo, this);
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

    public DamageCalculationInfo BuildDamageCalculationInfo(SkillInfo skillInfo)
    {
        return  new DamageCalculationInfo
        {
            DamageElement = ElementOverride ?? 
                            (skillInfo.Skill != null? skillInfo.Skill.Element : Element.None),
            DamageType = DamageType,
            Source = skillInfo.Source,
            Target = skillInfo.Target
        };
    }

    public List<Func<EffectResult>> BuildOverrides(Battler source, Battler target)
    {
        var list = new List<Func<EffectResult>>();
        var sourcePassiveEffects = source.PassiveEffects.Where(pe => pe is IDealDamageOverride);
        var targetPassiveEffects = target.PassiveEffects.Where(pe => pe is IReceiveDamageOverride);
        
        
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
    
    public override object Clone()
    {
        return new DamageEffect
        {
            ElementOverride = ElementOverride,
            DamageType = DamageType,
            DamageFactor = DamageFactor
        };
    }
    
    #region Interfaces
    public interface IDealDamagePassiveEffect
    {
        void BeforeDeal(SkillInfo skillInfo, ref int finalDamage);
    }
    
    public interface IDealDamageOverride
    {
        [CanBeNull] EffectResult OverrideDealDamage(SkillInfo skillInfo, DamageEffect damageEffect);
    }
    
    public interface IReceiveDamageOverride
    {
        [CanBeNull] EffectResult OverrideReceiveDamage(SkillInfo skillInfo, DamageEffect damageEffect);
    }

    public interface IDamageCalculationInfoOverride
    {
        void OverrideDamageCalculationInfo(ref DamageCalculationInfo damageCalculationInfo);
    }
    
    public interface IReceiveDamagePassiveEffect
    {
        void BeforeReceive(SkillInfo skillInfo, ref int finalDamage);
    }
    #endregion
    
}














