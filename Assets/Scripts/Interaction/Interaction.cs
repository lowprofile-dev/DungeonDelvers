using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public abstract class Interaction
{
    public abstract void Run(Interactable source);
    public virtual void Cleanup() { }
    public abstract IEnumerator Completion { get; }
}

