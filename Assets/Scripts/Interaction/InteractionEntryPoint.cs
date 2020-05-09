using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using XNode;

[InteractableNode(defaultNodeName = "Entry Point")]
public class InteractionEntryPoint : Node
{
    public enum EntryPointType
    {
        Startup,
        Active
    }

    public EntryPointType entryPointType = EntryPointType.Active;
    [Output(ShowBackingValue.Always), SerializeField] public FlowControl Entry;

    public IEnumerator Run(Interactable source)
    {
        yield return RunInteraction(source);
    }

    private IEnumerator RunInteraction(Interactable source)
    {
        var node = GetOutputPort("Entry").Connection.node as InteractionBase;

        while (node != null)
        {
            if (node is Interaction interaction)
            {
                var operation =  interaction.PerformInteraction(source);
                if (operation != null)
                {
                    source.CurrentInteraction = interaction;
                    if (interaction.waitForCompletion)
                        yield return operation;
                    else
                        source.StartCoroutine(operation);
                }
            }
            source.CurrentInteraction = null;
            node = node.GetNextNode();
        }
    }
}
