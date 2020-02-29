using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using SkredUtils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Random = System.Random;

public class GameController : AsyncMonoBehaviour
{
    public static GameController Instance { get; private set; }
    public GameObject AnimationObjectBase;
    public GameObject PlayerPrefab;
    public GameObject CameraPrefab;
    public GameObject MainCanvasPrefab;
    public GameObject GraphyPrefab;
    
    public Random Random;

    public int? TransitionSource = null;
    
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
        
        if (PlayerController.Instance == null)
            Instantiate(PlayerPrefab);
        if (TrackPlayer.Instance == null)
            Instantiate(CameraPrefab);
        if (MainCanvas.Instance == null)
            Instantiate(MainCanvasPrefab);
        
        Random = new Random();
        
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        Instantiate(GraphyPrefab);
#endif
    }

    public Dictionary<string, int> Globals = new Dictionary<string, int>();
    public Dictionary<int,int> Seeds = new Dictionary<int,int>();

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