using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetKeyInteraction : Interaction
{
    public ValueInteractableCondition.ValueScope Scope;
    public Operation operation;
    public string Key = "";
    public int Value = 0;
    public override void Run(Interactable source)
    {
        if (Scope == ValueInteractableCondition.ValueScope.Global)
        {
            switch (operation)
            {
                case Operation.Set:
                    GameController.SetGlobal(Key,Value);
                    break;
                case Operation.Increase:
                    GameController.SetGlobal(Key,GameController.GetGlobal(Key)+Value);
                    break;    
                case Operation.Decrease:
                    GameController.SetGlobal(Key,GameController.GetGlobal(Key)-Value);
                    break;
            }
        }
        else
        {
            switch (operation)
            {
                case Operation.Set:
                    source.SetLocal(Key,Value);
                    break;
                case Operation.Increase:
                    source.SetLocal(Key,source.GetLocal(Key)+Value);
                    break;    
                case Operation.Decrease:
                    source.SetLocal(Key,source.GetLocal(Key)-Value);
                    break;
            }
        }
    }

    public override IEnumerator Completion => null;

    public enum Operation
    {
        Set,
        Increase,
        Decrease
    }
}