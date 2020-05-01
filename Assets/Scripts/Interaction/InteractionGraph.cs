using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using XNode;

[CreateAssetMenu(menuName = "Interaction Graph")]
public class InteractionGraph : NodeGraph
{
    public Interactable source { get; private set; }
    
    public IEnumerator Run(InteractionEntryPoint.EntryPointType entryPointType, Interactable source)
    {
        this.source = source;
        var entryPoint = nodes.FirstOrDefault(node => node is InteractionEntryPoint _entryPoint && _entryPoint.entryPointType == entryPointType) as InteractionEntryPoint;
        if (entryPoint != null)
        {
            yield return entryPoint.Run(source);
        }
    }
}