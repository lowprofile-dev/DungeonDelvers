using System;
using Sirenix.OdinInspector;

public class CounterPassiveEffect : PassiveEffect, DamageEffect.IReceiveDamageOverride
{
    public DamageEffect DamageEffect = new DamageEffect();
    public DamageType? CounterType;
    [PropertyRange(0f,1f)] public float Chance;
    
    //Botar animações futuramente
    public EffectResult OverrideReceiveDamage(SkillInfo skillInfo, DamageEffect damageEffect)
    {
        //Não counterar caso não seja de uma skill (como outro counter)
        if (skillInfo.Skill == null)
            return null;

        //Não counterar se for do mesmo time
        if (BattleController.Instance.IsSameTeam(skillInfo.Source, skillInfo.Target))
            return null;
        
        //Não counterar se o dano não for do tipo necessario.
        if (CounterType.HasValue && DamageEffect.DamageType != CounterType.Value)
            return null;

        var rng = GameController.Instance.Random.NextDouble();
        if (rng < Chance)
            return new CounterEffectResult
            {
                CounterDamageEffect = DamageEffect,
                skillInfo = skillInfo
            };
        return null;
    }

    public class CounterEffectResult : EffectResult
    {
        public DamageEffect CounterDamageEffect;
    }
}
