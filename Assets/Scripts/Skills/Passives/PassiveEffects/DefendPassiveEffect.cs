using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class DefendPassiveEffect : PassiveEffect, DamageEffect.IReceiveDamagePassiveEffect, ITurnStartPassiveEffect
{
    public float DamageFactor;
    
    public void BeforeReceive(SkillInfo skillInfo, ref int finalDamage)
    {
        if (skillInfo.Target.BattleDictionary.TryGetValue("Defending", out var defending) && (bool) defending)
        {
            var old = finalDamage;
            finalDamage = (int) (finalDamage * DamageFactor);
            Debug.Log($"Aplicando DefendPassiveEffect -- Source: {skillInfo.Source.BattlerName}, Target: {skillInfo.Target.BattlerName}. {old} -> {finalDamage}");
        }
    }

    public async Task OnTurnStart(PassiveEffectInfo passiveEffectInfo)
    {
        var battler = passiveEffectInfo.Target;
        Debug.Log($"{battler.BattlerName} -> [Defending]=false");
        battler.BattleDictionary["Defending"] = false;
    }
}