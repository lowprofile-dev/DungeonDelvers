using System.Collections;
using XNode;

[InteractableNode(defaultNodeName = "Util/Self Reference")]
public class GetSelfRefInteraction : Interaction
{
    [Output] public Interactable Owner;
    
    public override IEnumerator PerformInteraction(Interactable source)
    {
        Owner = source;
        yield break;
    }

    public override object GetValue(NodePort port)
    {
        if (port.fieldName == "Owner")
        {
            return Owner;
        }
        return null;
    }
}
