using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkredUtils;
using UnityEngine;

public class MultipleInteraction : Interaction
{
    public List<Interaction> Interactions = new List<Interaction>();
    [HideInInspector] public List<Ref<bool>> Completed = new List<Ref<bool>>();

    public override void Run(Interactable source)
    {
        foreach (var interaction in Interactions)
        {
            interaction.Run(source);
            var completion = interaction.Completion;
            if (completion != null)
                source.StartCoroutine(WaitForInteractionCoroutine(completion));
        }
    }

    private IEnumerator WaitForInteractionCoroutine(IEnumerator interactionCompletion)
    {
        var completion = false.CreateRef();

        Completed.Add(completion);

        yield return interactionCompletion;

        completion.Instance = true;
    }

    public override IEnumerator Completion
    {
        get
        {
            while (Completed.Any(@ref => !@ref.Instance))
                yield return null;

            UnityEngine.Debug.Log("Acabou Todos");
        }
    }
}

