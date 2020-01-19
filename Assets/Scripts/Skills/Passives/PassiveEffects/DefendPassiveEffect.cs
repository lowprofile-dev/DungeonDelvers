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
            Debug.Log($"Aplicando DefendPassiveEffect -- Source: {skillInfo.Source.Name}, Target: {skillInfo.Target.Name}. {old} -> {finalDamage}");
        }
    }

    public async Task OnTurnStart(IBattler battler)
    {
        Debug.Log($"{battler.Name} -> [Defending]=false");
        battler.BattleDictionary["Defending"] = false;
    }
}