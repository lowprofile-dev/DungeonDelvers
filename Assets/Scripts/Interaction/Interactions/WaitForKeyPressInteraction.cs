using System.Collections;
using UnityEngine;

[InteractableNode(defaultNodeName = "Wait for Keypress")]
public class WaitForKeyPressInteraction : Interaction
{
    [Input] public KeyCode Key;
    
    public override IEnumerator PerformInteraction(Interactable source)
    {
        var key = GetInputValue("Key", Key);

        while (!Input.GetKeyDown(key))
            yield return null;
    }
}
