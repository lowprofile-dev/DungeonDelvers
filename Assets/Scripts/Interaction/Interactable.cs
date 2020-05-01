using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using XNode;

public class Interactable : SerializedMonoBehaviour
{
    public InteractableType interactableType = InteractableType.Action;
    public InteractionGraph Interactions;
    public Dictionary<string, object> InstanceVars = new Dictionary<string, object>();
    [ReadOnly] public bool IsInteracting = false;
    private void Awake()
    {
        if (Interactions == null)
            Interactions = GetComponent<InteractionSet>().graph;
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
        StartCoroutine(InteractionCoroutine(InteractionEntryPoint.EntryPointType.Startup));
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

    public object GetInstance(string key, object defaultIfNull)
    {
        if (!InstanceVars.ContainsKey(key) || InstanceVars[key] == null)
            return defaultIfNull;
        return InstanceVars[key];
    }
    
    public void SetInstance(string key, object value)
    {
        InstanceVars[key] = value;
    }

    public void Interact()
    {
        if (!IsInteracting)
            StartCoroutine(InteractionCoroutine(InteractionEntryPoint.EntryPointType.Active));
    }

    private IEnumerator InteractionCoroutine(InteractionEntryPoint.EntryPointType entryPointType)
    {
        if (Interactions == null)
            yield break;
        StartInteraction();
        yield return Interactions.Run(entryPointType, this);
        EndInteraction();
    }

    private void OnDestroy()
    {
        if (IsInteracting)
            EndInteraction();
        
        //UploadLocals();
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
