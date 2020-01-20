using System.Collections;
using UnityEngine;

public class TeleportInteraction : Interaction
{
    public Transform target;
    
    public override void Run(Interactable source)
    {
        PlayerController.Instance.transform.position = target.position;
        TrackPlayer.Instance.transform.position = target.position + new Vector3(0, 0, -10);
    }

    public override IEnumerator Completion  => null;
}