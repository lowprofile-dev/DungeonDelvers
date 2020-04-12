using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class Interactable : SerializedMonoBehaviour
{
    public InteractableType interactableType = InteractableType.Action;
    public List<Interaction> StartupInteractions = new List<Interaction>();
    public List<Interaction> Interactions = new List<Interaction>();
    [ReadOnly] public bool IsInteracting = false;
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
        StartCoroutine(_Interact(StartupInteractions, null));
    }

    private string LocalKeyToGlobalKey(string localKey)
    {
        return $"LOCAL{name}_{localKey}";
    }
    
    public int GetLocal(string key)
    {
        var globalKey = LocalKeyToGlobalKey(key);
        return GameController.GetGlobal(globalKey);
    }

    public void SetLocal(string key, int value)
    {
        var globalKey = LocalKeyToGlobalKey(key);
        GameController.SetGlobal(globalKey,value);
    }

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
            interaction.Run(source);
            var completion = interaction.Completion;
            if (completion != null && interaction.SkipWaiting == false)
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
            if (completion != null && interaction.SkipWaiting == false)
                yield return completion;
            interaction.Cleanup();
        }
        
        EndInteraction();
    }

    private void OnDestroy()
    {
        if (IsInteracting)
            EndInteraction();
        
        //UploadLocals();
    }

    public void UploadLocals()
    {
//        foreach (var local in Locals)
//        {
//            var globalName = $"LOCAL{name}_{local.Key}";
//            GameController.SetGlobal(globalName, local.Value);
//            Debug.Log($"Saving local {local} ({globalName})");
//        }
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
