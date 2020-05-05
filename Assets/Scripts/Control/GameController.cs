using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using E7.Introloop;
using Sirenix.OdinInspector;
using SkredUtils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Random = System.Random;

public class GameController : AsyncMonoBehaviour
{
    public static GameController Instance { get; private set; }
    public int targetFrameRate = -1;
    
    [HideInInspector] public Random Random;
    public AudioSource SfxPlayer;

    public int? TransitionSource = null;
    
    [HideInInspector] public UnityEvent OnBeginEncounter;
    [HideInInspector] public UnityEvent OnEndEncounter;

    public float GlobalPriceModifier = 1f;
    public float GlobalExperienceModifier = 1f;
    
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
            Instantiate(GameSettings.Instance.PlayerPrefab);
        if (TrackPlayer.Instance == null)
            Instantiate(GameSettings.Instance.CameraPrefab);
        if (MainCanvas.Instance == null)
            Instantiate(GameSettings.Instance.MainCanvasPrefab);
        
        Random = new Random();

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        Instantiate(GameSettings.Instance.GraphyPrefab);
        Application.targetFrameRate = targetFrameRate;
#endif
        
        this.Ensure(ref SfxPlayer);
        SfxPlayer.outputAudioMixerGroup = GameSettings.Instance.SFXChannel;
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