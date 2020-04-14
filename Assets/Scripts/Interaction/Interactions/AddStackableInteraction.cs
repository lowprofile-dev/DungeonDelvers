using System.Collections;

[InteractableNode(defaultNodeName = "Add Stackable")]
public class AddStackableInteraction : Interaction
{
    [Input] private IStackableBase StackableBase;
    [Input] public int Quantity;
    
    public override IEnumerator PerformInteraction(Interactable source)
    {
        PlayerController.Instance.AddStackableBaseToInventory(GetInputValue("StackableBase",StackableBase), GetInputValue("Quantity",Quantity));
        yield break;
    }
}
