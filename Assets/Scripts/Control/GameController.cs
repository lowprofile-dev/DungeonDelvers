using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class GameController : SerializedMonoBehaviour
{
    public static GameController Instance { get; private set; }
    public TrackTransform TrackTransform;

    [HideInInspector] public UnityEvent OnBeginEncounter;
    [HideInInspector] public UnityEvent OnEndEncounter;

    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        TrackTransform = GetComponent<TrackTransform>();
        TrackTransform.Target = PlayerController.Instance.gameObject.transform;
    }

    public Dictionary<string, int> Globals = new Dictionary<string, int>();

    public static int GetGlobal(string key)
    {
        if (Instance == null)
            throw new NullReferenceException();

        if (!Instance.Globals.ContainsKey(key))
            Instance.Globals[key] = 0;
        return Instance.Globals[key];
    }

    public static void SetGlobal(string key, int value)
    {
        if (Instance == null)
            throw new NullReferenceException();

        Instance.Globals[key] = value;
    }
    
    private Queue<Action> QueuedActions = new Queue<Action>();

    public async Task QueueActionAndAwait(Action action)
    {
        var completed = false;
        var fullAction = (action += () => completed = true);
        
        QueuedActions.Enqueue(fullAction);
        
        //refazer depois, primeiro fazer funcionar
        while (!completed)
        {
            await Task.Delay(5);
        }
    }

    public void QueueAction(Action action)
    {
        QueuedActions.Enqueue(action);
    }

    private void LateUpdate()
    {
        lock (QueuedActions)
        {
            while (QueuedActions.Any())
            {
                var action = QueuedActions.Dequeue();
                action.Invoke();
            }
        }
    }
}
