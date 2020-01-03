using System.Collections;
using UnityEngine;

public class CallInteractionInteractable : Interaction
{
    public Interactable Interactable;
    public override void Run(Interactable source)
    {
        Interactable.Interact();
    }

    public override IEnumerator Completion => new WaitUntil(() => !Interactable.IsInteracting);
}