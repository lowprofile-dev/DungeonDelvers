using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using SkredUtils;
using UnityEngine;
using UnityEngine.Events;

public class GameController : AsyncMonoBehaviour
{
    public static GameController Instance { get; private set; }
    public GameObject AnimationObjectBase;

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
            return;

        Instance.Globals[key] = value;
    }
}