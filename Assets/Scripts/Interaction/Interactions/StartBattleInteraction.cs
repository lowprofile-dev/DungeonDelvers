using System.Collections;
using UnityEngine;

[InteractableNode(defaultNodeName = "Start Battle")]
public class StartBattleInteraction : Interaction
{
    [Input] public EncounterSet EncounterSet;
    [Input] public Sprite BattlegroundSprite;

    public override IEnumerator PerformInteraction(Interactable source)
    {
        var encounterSet = GetInputValue("EncounterSet", EncounterSet);
        var battlegroundSprite = GetInputValue("BattlegroundSprite", BattlegroundSprite);

        var finished = false;
        void stopWaiting() => finished = true;
        
        BattleController.Instance.OnBattleEnd.AddListener(stopWaiting);
        BattleController.Instance.BeginBattle(encounterSet,battlegroundSprite);
        
        yield return new WaitWhile((() => !finished));
        yield return null;
        
        BattleController.Instance.OnBattleEnd.RemoveListener(stopWaiting);
    }
}
