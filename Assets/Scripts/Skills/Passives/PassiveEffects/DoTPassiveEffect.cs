using System.Threading.Tasks;

//Arrumar, ver como a source ser o personagem que aplicou o DoT
public class DoTPassiveEffect : PassiveEffect, ITurnStartPassiveEffect, IHasSource
{
    public DamageEffect DamageEffect;
    public IBattler Source { get; set; }

    public async Task OnTurnStart(IBattler battler)
    {
        battler.QueueAction(() =>
            BattleController.Instance.battleCanvas.battleInfoPanel.ShowInfo(PassiveSource.GetName));
        await battler.ReceiveEffect(Source, null, DamageEffect);
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
