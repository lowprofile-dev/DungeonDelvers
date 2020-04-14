using System.Collections;
using UnityEngine;

[InteractableNode(defaultNodeName = "Util/Toggle Component")]
public class ToggleComponentInteraction : Interaction
{
    [Input] public MonoBehaviour Component;
    [Input] public bool Active = true;
    
    public override IEnumerator PerformInteraction(Interactable source)
    {
        var component = GetInputValue("Component", Component);
        var active = GetInputValue("Active", Active);

        component.enabled = active;
        
        yield break;
    }
}
