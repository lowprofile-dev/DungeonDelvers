using System.Collections;
using System.Collections.Generic;

public class AddStackableInteraction : Interaction
{
    public IStackableBase StackableBase;
    public int Quantity;
    public override void Run(Interactable source)
    {
        PlayerController.Instance.AddStackableToInventory(StackableBase,Quantity);
    }

    public override IEnumerator Completion => null;
}
