using System.Threading.Tasks;

public class DoTPassiveEffect : PassiveEffect, ITurnStartPassiveEffect
{
    public DamageEffect DamageEffect;

    public async Task OnTurnStart(PassiveEffectInfo passiveEffectInfo)
    {
        var source = passiveEffectInfo.Source;
        var target = passiveEffectInfo.Target;
        
        target.QueueAction(() =>
            BattleController.Instance.battleCanvas.battleInfoPanel.ShowInfo(passiveEffectInfo.PassiveEffectSourceName));
        
        //await battler.ReceiveEffect(Source, null, DamageEffect);
        await target.ReceiveEffect(new EffectInfo
        {
            SkillInfo = new SkillInfo
            {
                HasCrit = false,
                Skill = null,
                Source = source,
                Target = target
            },
            Effect = DamageEffect
        });
        
        target.QueueAction(() =>
        {
            BattleController.Instance.battleCanvas.battleInfoPanel.HideInfo();
        });
    }
}
