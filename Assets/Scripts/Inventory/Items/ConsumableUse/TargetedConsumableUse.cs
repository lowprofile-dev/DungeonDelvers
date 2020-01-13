using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class TargetedConsumableUse : ConsumableUse
{
    [HideInInspector] public Character Target { get; set; } = null;

    public sealed override IEnumerator ApplyUse()
    {
        if (Target == null)
            throw new NullReferenceException();

        yield return ApplyToCharacter(Target);
    }

    public abstract IEnumerator ApplyToCharacter(Character character);
}

