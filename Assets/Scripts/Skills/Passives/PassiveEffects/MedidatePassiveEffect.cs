using System.Threading.Tasks;
using UnityEngine;

public class MeditatePassiveEffect : PassiveEffect, ITurnStartPassiveEffect
{
    public int EpGain = 0;

    public async Task OnTurnStart(PassiveEffectInfo passiveEffectInfo)
    {
        var effect = new GainApEffect
        {
            ApAmount = EpGain
        };
        
        passiveEffectInfo.Target.QueueAction(() =>
        {
            BattleController.Instance.battleCanvas.battleInfoPanel.ShowInfo(passiveEffectInfo.PassiveEffectSourceName);
        });

        Debug.Log($"{passiveEffectInfo.Source.BattlerName} deu {EpGain} EP para {passiveEffectInfo.Target.BattlerName}");

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
        });
    }
}