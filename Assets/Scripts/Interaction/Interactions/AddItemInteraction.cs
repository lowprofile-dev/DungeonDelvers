using System.Collections;

[InteractableNode(defaultNodeName = "Add Item")]
public class AddItemInteraction : Interaction
{
    [Input] public ItemBase ItemBase;
    
    public override IEnumerator PerformInteraction(Interactable source)
    {
        PlayerController.Instance.AddItemBaseToInventory(GetInputValue("ItemBase", ItemBase));
        yield break;
    }
}
