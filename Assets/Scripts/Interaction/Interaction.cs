using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using JetBrains.Annotations;
using XNode;

public abstract class Interaction : InteractionBase
{
    [Input(ShowBackingValue.Never)]
    public FlowControl Entry;
    [Output(connectionType = ConnectionType.Override)]
    public FlowControl Exit;

    public override InteractionBase GetNextNode()
    {
        var exitPort = GetOutputPort("Exit")?.Connection?.node as InteractionBase;
        return exitPort;
    }
    
    [CanBeNull] public abstract IEnumerator PerformInteraction(Interactable source);
}

[Serializable]
public sealed class  FlowControl { }