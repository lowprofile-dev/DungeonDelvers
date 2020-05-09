using System;
using System.Collections;
using UnityEngine;

[InteractableNode(defaultNodeName = "Inn")]
public class InnInteraction : Interaction
{
    public GameObject InnMenuPrefab;
    public int InnCost;
    
    private void Reset()
    {
        InnMenuPrefab = GameSettings.Instance.DefaultInnMenu;
    }

    public override IEnumerator PerformInteraction(Interactable source)
    {
        var innObject = Instantiate(InnMenuPrefab);
        var innComponent = innObject.GetComponent<InnMenu>();
        var isClosed = innComponent.Initialize(InnCost);
        
        yield return new WaitUntil(() => isClosed);
    }
}
