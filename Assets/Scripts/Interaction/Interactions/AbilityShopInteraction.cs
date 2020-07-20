using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

[InteractableNode(defaultNodeName = "Ability Shop")]
public class AbilityShopInteraction : Interaction
{
    public GameObject AbilityShopCanvas;

    private void Reset()
    {
        AbilityShopCanvas = GameSettings.Instance.DefaultAbilityShopMenu;
    }

    public override IEnumerator PerformInteraction(Interactable source)
    {
        var abilityShopObject = Instantiate(AbilityShopCanvas);
        var abilityShopComponent = abilityShopObject.GetComponent<AbilityShopMenu>();
        var isClosed = abilityShopComponent.Open();

        yield return new WaitUntil(() => isClosed.Instance);
    }
}
