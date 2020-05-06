using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

public class MeditatePassiveEffect : PassiveEffect, ITurnStartPassiveEffect
{
    public int ApGain = 0;
    public DD.Skill.Animation.Animation Animation = null; 

    public async Task OnTurnStart(PassiveEffectInfo passiveEffectInfo)
    {
        var effect = new GainApEffect
        {
            ApAmount = ApGain
        };
        
        passiveEffectInfo.Target.QueueAction(() =>
        {
            BattleController.Instance.battleCanvas.battleInfoPanel.ShowInfo(passiveEffectInfo.PassiveEffectSourceName);
        });

        if (Animation != null)
        {
            await Animation.PlaySkillAnimation(passiveEffectInfo.Source, new []{passiveEffectInfo.Target});
        }
        
        Debug.Log($"{passiveEffectInfo.Source.BattlerName} deu {ApGain} EP para {passiveEffectInfo.Target.BattlerName}");

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