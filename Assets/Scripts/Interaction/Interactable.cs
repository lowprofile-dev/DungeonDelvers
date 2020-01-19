using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Interactable : SerializedMonoBehaviour
{
    public InteractableType interactableType = InteractableType.Action;
    public List<Interaction> StartupInteractions = new List<Interaction>();
    public List<Interaction> Interactions = new List<Interaction>();
    [ReadOnly] public bool IsInteracting = false;
    
    public Dictionary<string, int> Locals = new Dictionary<string, int>();

    private void Awake()
    {
        var originalName = name;
        var appendedName = $"{SceneManager.GetActiveScene().buildIndex}_{originalName}";

        if (GameObject.Find(appendedName) == null)
        {
            name = appendedName;
        }
        else
        {
            for (int i = 0; i < 9999; i++)
            {
                var attempt = $"{appendedName}_{i}";
                if (GameObject.Find(attempt) == null)
                {
                    name = attempt;
                    break;
                }

            }
        }
    }

    private void Start()
    {
        ReloadLocals();
        StartCoroutine(_Interact(StartupInteractions, null));
    }

    private void ReloadLocals()
    {
        var key = $"LOCAL{name}_";
        var keyLength = key.Length;

        var locals = GameController.Instance.Globals.Keys.Where(globalKey => globalKey.StartsWith(key));

        foreach (var local in locals)
        {
            var value = GameController.GetGlobal(local);
            var localName = local.Remove(0, keyLength);

            Locals[localName] = value;

            Debug.Log($"Reloading local {localName} ({local})");
        }
    }

    public int GetLocal(string key)
    {
        if (!Locals.ContainsKey(key))
            Locals[key] = 0;
        return Locals[key];
    }

    public void SetLocal(string key, int value) => Locals[key] = value;

    public void Interact(bool isNested = false, Interactable parent = null)
    {
        if (isNested)
        {
            StartCoroutine(_NestedInteraction(Interactions, parent));
        }
        else if (!IsInteracting)
        {
            StartCoroutine(_Interact(Interactions, parent));
        }
    }

    IEnumerator _NestedInteraction(IEnumerable<Interaction> interactions, Interactable parent)
    {
        IsInteracting = true;
        var source = parent != null ? parent : this;
        foreach (var interaction in interactions)
        {
            //yield return null;
            interaction.Run(source);
            var completion = interaction.Completion;
            if (completion != null)
                yield return completion;
            interaction.Cleanup();
        }

        IsInteracting = false;
    }
    
    IEnumerator _Interact(IEnumerable<Interaction> interactions, Interactable parent)
    {
        StartInteraction();

        var source = parent != null ? parent : this;
        foreach (var interaction in interactions)
        {
            //yield return null;
            interaction.Run(source);
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
        
        foreach (var local in Locals)
        {
            var globalName = $"LOCAL{name}_{local.Key}";
            GameController.SetGlobal(globalName, local.Value);
            Debug.Log($"Saving local {local} ({globalName})");
        }
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
