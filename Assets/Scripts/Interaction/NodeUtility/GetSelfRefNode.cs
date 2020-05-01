using XNode;

[InteractableNode(defaultNodeName = "Util/Self Reference")]
public class GetSelfRefNode : Node
{
    [Output] public Interactable Owner;
    
    public override object GetValue(NodePort port)
    {
        if (port.fieldName == "Owner")
        {
            return (graph as InteractionGraph).source;
        }
        return null;
    }
}
