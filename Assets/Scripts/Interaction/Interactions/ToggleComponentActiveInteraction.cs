using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleComponentActiveInteraction : Interaction
{
    public MonoBehaviour Component;
    public bool Active = true;

    public override void Run(Interactable source)
    {
        Component.enabled = Active;
    }
    
    public override IEnumerator Completion => null;
}