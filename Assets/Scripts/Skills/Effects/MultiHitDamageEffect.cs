using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;

public class MultiHitDamageEffect : Effect
{
    public List<HitInfo> HitInfo = new List<HitInfo>();

    public override EffectResult ExecuteEffect(SkillInfo skillInfo)
    {
        if (HitInfo == null || !HitInfo.Any())
            throw new ArgumentException();

        var hitResults = new List<HitResult>();

        HitInfo.ForEach((info, i) =>
        {
            //throw new NotImplementedException();
            var damage = (int)(Mathf.Max(0,BattleController.Instance.DamageCalculation(
                                               new DamageEffect.DamageCalculationInfo
                                               {
                                                   Source = skillInfo.Source, 
                                                   Target = skillInfo.Target, 
                                                   DamageElement = skillInfo.Skill.Element, 
                                                   DamageType = info.DamageType
                                               }) * info.DamageFactor));

            Debug.Log($"Calculado hit {i+1}: {damage}");
        
            var targetPassives = skillInfo.Target.Passives
                .SelectMany(passive => passive.Effects.Where(passiveEffect => passiveEffect is DamageEffect.IReceiveDamagePassiveEffect))
                .OrderByDescending(effect => effect.Priority)
                .Cast<DamageEffect.IReceiveDamagePassiveEffect>()
                .ToArray();

            targetPassives.ForEach(targetPassive => targetPassive.BeforeReceive(skillInfo, ref damage));
        
            Debug.Log($"{skillInfo.Source} causou {damage} de dano em {skillInfo.Target}.");

            hitResults.Add(new HitResult
            {
                DamageDealt = damage,
                DamageType = info.DamageType
            });
        });

        var result = new MultiHitDamageEffectResult
        {
            HitResults = hitResults,
            skillInfo = skillInfo
        };

        skillInfo.Target.CurrentHp -= result.TotalDamageDealt;

        return result;
    }

    public class MultiHitDamageEffectResult : EffectResult
    {
        public List<HitResult> HitResults;
        public int TotalDamageDealt => HitResults.Select(result => result.DamageDealt).Sum();
    }
}

public struct HitInfo
{
    public DamageType DamageType;
    public float DamageFactor;
}

public struct HitResult
{
    public DamageType DamageType;
    public int DamageDealt;
}
