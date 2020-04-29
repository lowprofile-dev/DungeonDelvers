﻿using UnityEngine;

public class DamageReductionPassiveEffect : PassiveEffect, DamageEffect.IReceiveDamagePassiveEffect
{
    public float DamageFactor;
    
    public void BeforeReceive(SkillInfo skillInfo, ref int finalDamage)
    {
        if (skillInfo.Target.BattleDictionary.TryGetValue("Defending", out var defending) && (bool) defending)
        {
            var old = finalDamage;
            finalDamage = (int) (finalDamage * DamageFactor);
            Debug.Log($"Aplicando DamageReductionPassiveEffect -- Source: {skillInfo.Source.BattlerName}, Target: {skillInfo.Target.BattlerName}. {old} -> {finalDamage}");
        }
    }
}