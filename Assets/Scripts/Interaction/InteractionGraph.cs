using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using XNode;

public class InteractionGraph : NodeGraph
{
    public IEnumerator Run(InteractionEntryPoint.EntryPointType entryPointType, Interactable source)
    {
        var entryPoint = nodes.FirstOrDefault(node => node is InteractionEntryPoint _entryPoint && _entryPoint.entryPointType == entryPointType) as InteractionEntryPoint;
        if (entryPoint != null)
        {
            yield return entryPoint.Run(source);
        }
    }
}