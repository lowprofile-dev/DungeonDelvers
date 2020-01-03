using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Interactable : SerializedMonoBehaviour
{
    public InteractableType interactableType = InteractableType.Action;
    public List<Interaction> StartupInteractions = new List<Interaction>();
    public List<Interaction> Interactions = new List<Interaction>();
    [ReadOnly] public bool IsInteracting = false;
    
    public Dictionary<string, int> Locals = new Dictionary<string, int>();

    private void Start()
    {
        StartCoroutine(_Interact(StartupInteractions));
    }

    public int GetLocal(string key)
    {
        if (!Locals.ContainsKey(key))
            Locals[key] = 0;
        return Locals[key];
    }

    public void SetLocal(string key, int value) => Locals[key] = value;

    public void Interact(bool isNested = false)
    {
        if (isNested)
        {
            StartCoroutine(_NestedInteraction(Interactions));
        }
        else if (!IsInteracting)
        {
            StartCoroutine(_Interact(Interactions));
        }
    }

    IEnumerator _NestedInteraction(IEnumerable<Interaction> interactions)
    {
        foreach (var interaction in interactions)
        {
            //yield return null;
            interaction.Run(this);
            var completion = interaction.Completion;
            if (completion != null)
                yield return completion;
            interaction.Cleanup();
        }
    }
    
    IEnumerator _Interact(IEnumerable<Interaction> interactions)
    {
        StartInteraction();

        foreach (var interaction in interactions)
        {
            //yield return null;
            interaction.Run(this);
            var completion = interaction.Completion;
            if (completion != null)
                yield return completion;
            interaction.Cleanup();
        }
        
        EndInteraction();
    }

    private void OnDestroy()
    {
        if (IsInteracting)
            EndInteraction();
    }

    private void StartInteraction()
    {
        //Time.timeScale = 0;
        IsInteracting = true;
        PlayerController.Instance.State = PlayerController.PlayerState.Busy;
    }

    private void EndInteraction()
    {
        //Time.timeScale = 1;
        IsInteracting = false;
        PlayerController.Instance.State = PlayerController.PlayerState.Active;
    }

    public enum InteractableType
    {
        Action,
        Collision
    }
}
