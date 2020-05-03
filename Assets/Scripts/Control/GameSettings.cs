using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameSettings : SerializedScriptableObject
{
    private static GameSettings instance;
    public static GameSettings Instance
    {
        get {
            if (instance == null)
            {
                instance = Resources.Load<GameSettings>("GameSettings");
                if (instance == null)
                {
#if UNITY_EDITOR
                    instance = CreateInstance<GameSettings>();
                    AssetDatabase.CreateAsset(instance, "Assets/Resources/GameSettings.asset");
#else
                Debug.LogError("Game Settings Missing.");
                Application.Quit();
#endif
                }

            }
            return instance;
        }
    }

    [TabGroup("Default Prefabs")] public GameObject AnimationObject;
    [TabGroup("Default Prefabs")] public GameObject PlayerPrefab;
    [TabGroup("Default Prefabs")] public GameObject CameraPrefab;
    [TabGroup("Default Prefabs")] public GameObject MainCanvasPrefab;
    [TabGroup("Default Prefabs")] public GameObject GraphyPrefab;

    [TabGroup("Audio Settings")] public AudioMixer AudioMixer;
    [TabGroup("Audio Settings")] public AudioMixerGroup BGMChannel;
    [TabGroup("Audio Settings")] public AudioMixerGroup SFXChannel;
    
}
