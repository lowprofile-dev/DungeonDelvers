using System.Threading.Tasks;

//Arrumar, ver como a source ser o personagem que aplicou o DoT
public class DoTPassiveEffect : PassiveEffect, ITurnStartPassiveEffect
{
    public DamageEffect DamageEffect;
    
    public async Task OnTurnStart(IBattler battler)
    {
        battler.QueueAction(() =>
            BattleController.Instance.battleCanvas.battleInfoPanel.ShowInfo(PassiveSource.GetName));
        await battler.ReceiveEffect(battler, null, DamageEffect);
        battler.QueueAction(() =>
        {
            BattleController.Instance.battleCanvas.battleInfoPanel.HideInfo();
            //BattleController.Instance.battleCanvas.UnbindActionArrow();
        });
    }
}
