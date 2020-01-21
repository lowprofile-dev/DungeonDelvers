﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class RegenPassiveEffect : PassiveEffect, ITurnStartPassiveEffect
{
    [HideIf("IsPercentageValue")] public int FlatValue = 0;
    [ShowIf("IsPercentageValue"), PropertyRange(0,1f)] public float PercentageValue = 0f;

    public bool IsPercentageValue = false;

    public async Task OnTurnStart(IBattler battler)
    {
        int healAmount;

        if (!IsPercentageValue)
        {
            healAmount = FlatValue;
        }
        else
        {
            healAmount = (int) (battler.Stats.MaxHp * PercentageValue);
        }

        var effect = new HealEffect
        {
            HealAmount = healAmount
        };

        battler.QueueAction(() =>
        {
            BattleController.Instance.battleCanvas.battleInfoPanel.ShowInfo(PassiveSource.PassiveName);
            //BattleController.Instance.battleCanvas.BindActionArrow(battler.RectTransform);
        });
        
        Debug.Log($"Curando {healAmount} em {battler.Name}");
        await battler.ReceiveEffect(battler, null, effect);
        
        battler.QueueAction(() =>
        {
            BattleController.Instance.battleCanvas.battleInfoPanel.HideInfo();
            //BattleController.Instance.battleCanvas.UnbindActionArrow();
        });
    }
}
