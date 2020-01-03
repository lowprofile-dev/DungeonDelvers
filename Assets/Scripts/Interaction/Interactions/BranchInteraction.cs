using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BranchInteraction : Interaction
{
    public List<Branch> Branches = new List<Branch>();
    public Interactable Default = null;
    private Interactable chosenInteractable;
    public override void Run(Interactable source)
    {
        chosenInteractable = Default;
        foreach (var branch in Branches)
        {
            if (branch.Condition.ConditionReached(source))
            {
                chosenInteractable = branch.Interactable;
                break;
            }
        }

        if (chosenInteractable != null)
            chosenInteractable.Interact();
    }

    public override IEnumerator Completion => new WaitUntil(() => chosenInteractable == null || !chosenInteractable.IsInteracting);

    [Serializable]
    public struct Branch
    {
        public ICondition Condition;
        public Interactable Interactable;
    }
}