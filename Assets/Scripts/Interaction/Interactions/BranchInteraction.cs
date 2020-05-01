using XNode;

[InteractableNode(defaultNodeName = "Branch2")]
public class BranchInteraction : InteractionBase
{
    [Input] public FlowControl Entry;
    [Input] public bool condition;
    [Output] public FlowControl IfTrue;
    [Output] public FlowControl IfFalse;
    
    
    public override InteractionBase GetNextNode()
    {
        var t1 = GetOutputPort("IfTrue");
        var t2 = t1?.Connection;
        var t3 = t2?.node;
        var t4 = t3 as InteractionBase;

        var f1 = GetOutputPort("IfFalse");
        var f2 = f1?.Connection;
        var f3 = f2?.node;
        var f4 = f3 as InteractionBase;

        return GetInputValue("condition", condition) ? 
            GetOutputPort("IfTrue")?.Connection?.node as InteractionBase :
            GetOutputPort("IfFalse")?.Connection?.node as InteractionBase;
    }
}
