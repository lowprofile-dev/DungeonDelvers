using System.Collections;
using UnityEngine;

public class StartBattleInteraction : Interaction
{
    public GameObject encounterPrefab;
    public Sprite battlegroundSprite;
    
    public override void Run(Interactable source)
    {
        BattleController.Instance.BeginBattle(encounterPrefab, battlegroundSprite);
    }

    public override IEnumerator Completion => new WaitWhile(() => BattleController.Instance.IsBattleOver() == 0);
}
