using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class RegenPassiveEffect : PassiveEffect, ITurnStartPassiveEffect
{
    //Alterar para ser um efeito de heal (que nem o DoTPassiveEffect usa dano)
    [HideIf("IsPercentageValue")] public int FlatValue = 0;
    [ShowIf("IsPercentageValue"), PropertyRange(0,1f)] public float PercentageValue = 0f;

    public bool IsPercentageValue = false;

    public async Task OnTurnStart(PassiveEffectInfo passiveEffectInfo)
    {
        int healAmount;

        if (!IsPercentageValue)
        {
            healAmount = FlatValue;
        }
        else
        {
            healAmount = (int) (passiveEffectInfo.Target.Stats.MaxHp * PercentageValue);
        }

        var effect = new HealEffect
        {
            HealAmount = healAmount
        };

        passiveEffectInfo.Target.QueueAction(() =>
        {
            BattleController.Instance.battleCanvas.battleInfoPanel.ShowInfo(passiveEffectInfo.PassiveEffectSourceName);
            //BattleController.Instance.battleCanvas.BindActionArrow(battler.RectTransform);
        });
        
        Debug.Log($"{passiveEffectInfo.Source.BattlerName} curou {healAmount} em {passiveEffectInfo.Target.BattlerName}");
        
        //await battler.ReceiveEffect(battler, null, effect);
        await passiveEffectInfo.Target.ReceiveEffect(new EffectInfo
        {
            SkillInfo = new SkillInfo
            {
                HasCrit = false,
                Skill = null,
                Source = passiveEffectInfo.Source,
                Target = passiveEffectInfo.Target
            },
            Effect = effect
        });
        
        passiveEffectInfo.Target.QueueAction(() =>
        {
            BattleController.Instance.battleCanvas.battleInfoPanel.HideInfo();
            //BattleController.Instance.battleCanvas.UnbindActionArrow();
        });
    }
}

