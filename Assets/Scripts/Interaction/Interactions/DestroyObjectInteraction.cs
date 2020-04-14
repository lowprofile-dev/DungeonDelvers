using System.Collections;
using UnityEngine;

[InteractableNode(defaultNodeName = "Destroy Object")]
public class DestroyObjectInteraction : Interaction
{
    [Input] public GameObject GameObject;
    
    public override IEnumerator PerformInteraction(Interactable source)
    {
        Destroy(GetInputValue("GameObject",GameObject));
        yield return null;
    }
}
