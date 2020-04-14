using System.Collections;
using UnityEngine;

[InteractableNode(defaultNodeName = "Teleport")]
public class TeleportInteraction : Interaction
{
    [Input] public Transform Target;
    
    public override IEnumerator PerformInteraction(Interactable source)
    {
        var target = GetInputValue("Target", Target);
        PlayerController.Instance.transform.position = target.position;
        TrackPlayer.Instance.transform.position = target.position + new Vector3(0, 0, -10);
        yield break;
    }
}
