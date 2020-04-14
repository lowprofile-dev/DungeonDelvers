using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using XNode;

[InteractableNode(defaultNodeName = "Log")]
public class LogInteraction : Interaction
{
    [Input] public string Text;

    public override IEnumerator PerformInteraction(Interactable source)
    {
        Debug.Log(GetInputValue<object>("Text", Text));
        yield break;
    }

    private void Reset()
    {
        name = "Log";
    }
}
