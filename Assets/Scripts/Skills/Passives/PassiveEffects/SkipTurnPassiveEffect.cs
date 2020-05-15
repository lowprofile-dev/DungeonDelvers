using System.Threading.Tasks;

public class SkipTurnPassiveEffect : PassiveEffect, IBuildTurnOverridePassiveEffect
{
    //condicões?

    public async Task<Turn> BuildTurnOverride(Battler battler)
    {
        battler.QueueAction(() =>
        {
            BattleController.Instance.battleCanvas.BindActionArrow(battler.RectTransform);
        });
            
        await BattleController.Instance.battleCanvas.battleInfoPanel.DisplayInfo("Stunned",1000);
            
        battler.QueueAction(() =>
        {
            BattleController.Instance.battleCanvas.UnbindActionArrow();
            BattleController.Instance.battleCanvas.CleanTargetArrows();
        });
        
        return new Turn{Skill = null};
    }
}
