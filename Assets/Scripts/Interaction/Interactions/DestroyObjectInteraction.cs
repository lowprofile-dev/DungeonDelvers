using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObjectInteraction : Interaction
{
    public UnityEngine.Object Object;
    public bool Immediate = false;

    public override void Run(Interactable source)
    {
        if (Immediate)
            UnityEngine.Object.DestroyImmediate(Object);
        else
            UnityEngine.Object.Destroy(Object);
    }

    public override IEnumerator Completion => null;
}