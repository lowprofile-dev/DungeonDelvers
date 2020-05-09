using System;
using System.Collections;
using UnityEngine;

[InteractableNode(defaultNodeName = "Forge")]
public class ForgeInteraction : Interaction
{
    public GameObject ForgeCanvas;
    
    private void Reset()
    {
        ForgeCanvas = GameSettings.Instance.DefaultForgeMenu;
    }

    public override IEnumerator PerformInteraction(Interactable source)
    {
        var forgeObject = Instantiate(ForgeCanvas);
        var forgeComponent = forgeObject.GetComponent<ForgeMenu>();
        var isClosed = forgeComponent.Open();
        
        yield return new WaitUntil(() => isClosed.Instance);
    }
}
