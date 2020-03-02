using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

public class DamageEffect : Effect
{
    public float DamageFactor = 1.0f;
    public DamageType DamageType = DamageType.Physical;

    public override EffectResult ExecuteEffect(SkillInfo skillInfo)
    {
        var damage = (int)(Mathf.Max(0,BattleController.Instance.DamageCalculation(skillInfo.Source, skillInfo.Target, DamageType)) * DamageFactor);

        Debug.Log($"Calculado {damage}");
        
        var targetPassives = skillInfo.Target.Passives
            .SelectMany(passive => passive.Effects.Where(passiveEffect => passiveEffect is IReceiveDamagePassiveEffect))
            .OrderByDescending(effect => effect.Priority)
            .Cast<IReceiveDamagePassiveEffect>()
            .ToArray();

        targetPassives.ForEach(targetPassive => targetPassive.BeforeReceive(skillInfo, ref damage));
        
        Debug.Log($"{skillInfo.Source} causou {damage} de dano em {skillInfo.Target}.");

        skillInfo.Target.CurrentHp -= damage;
        return new DamageEffectResult()
        {
            DamageDealt = damage,
            skillInfo = skillInfo
        };
    }

    public class DamageEffectResult : EffectResult
    {
        public int DamageDealt;
    }

    public interface IDealDamagePassiveEffect
    {
        void BeforeDeal(SkillInfo skillInfo, ref int finalDamage);
    }
    
    public interface IReceiveDamagePassiveEffect
    {
        void BeforeReceive(SkillInfo skillInfo, ref int finalDamage);
    }
}

public enum DamageType
{
    Physical,
    Magical,
    Pure
}