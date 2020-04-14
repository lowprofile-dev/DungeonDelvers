using XNode;

[InteractableNode(defaultNodeName = "Branch")]
public class BranchInteraction : InteractionBase
{
    [Input] public bool condition;
    [Output] public bool @true;
    [Output] public bool @false;
    
    
    public override InteractionBase GetNextNode()
    {
        return GetInputValue<bool>("condition", condition) ?
            GetOutputPort("@true")?.Connection?.node as InteractionBase : 
            GetOutputPort("@false")?.Connection?.node as InteractionBase;
    }
}
