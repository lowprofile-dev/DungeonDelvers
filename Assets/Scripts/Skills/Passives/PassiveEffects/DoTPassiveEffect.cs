using System.Threading.Tasks;

public class DoTPassiveEffect : PassiveEffect, ITurnStartPassiveEffect, IHasSource
{
    public DamageEffect DamageEffect;
    public IBattler Source { get; set; }

    public async Task OnTurnStart(IBattler battler)
    {
        battler.QueueAction(() =>
            BattleController.Instance.battleCanvas.battleInfoPanel.ShowInfo(PassiveSource.GetName));
        
        //await battler.ReceiveEffect(Source, null, DamageEffect);
        await battler.ReceiveEffect(new EffectInfo
        {
            SkillInfo = new SkillInfo
            {
                HasCrit = false,
                Skill = null,
                Source = Source,
                Target = battler
            },
            Effect = DamageEffect
        });
        
        battler.QueueAction(() =>
        {
            BattleController.Instance.battleCanvas.battleInfoPanel.HideInfo();
            //BattleController.Instance.battleCanvas.UnbindActionArrow();
        });
    }

    public override PassiveEffect GetInstance()
    {
        var instance = new DoTPassiveEffect
        {
            DamageEffect = DamageEffect,
            PassiveSource = PassiveSource,
            Priority = Priority,
            Source = Source
        };

        return instance;
    }
}
