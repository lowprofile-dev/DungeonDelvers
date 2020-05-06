using System.Collections.Generic;
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

    [TabGroup("1", "Default Prefabs")] public GameObject AnimationObject;
    [TabGroup("1", "Default Prefabs")] public GameObject PlayerPrefab;
    [TabGroup("1", "Default Prefabs")] public GameObject CameraPrefab;
    [TabGroup("1", "Default Prefabs")] public GameObject MainCanvasPrefab;
    [TabGroup("1", "Default Prefabs")] public GameObject GraphyPrefab;

    [TabGroup("1", "Audio Settings")] public AudioMixer AudioMixer;
    [TabGroup("1", "Audio Settings")] public AudioMixerGroup BGMChannel;
    [TabGroup("1", "Audio Settings")] public AudioMixerGroup SFXChannel;
    [TabGroup("1", "Audio Settings")] public AudioMixerGroup TextChannel;

    [TabGroup("1", "Battle Settings")] public SoundInfo DefaultMonsterHitSound;
    [TabGroup("1", "Battle Settings")] public SoundInfo DefaultFanfare;
    [TabGroup("1", "Battle Settings")] public int InitialAp;
    [TabGroup("1", "Battle Settings")] public int ApGain;
    
    [TabGroup("2","Message Box")] public GameObject DefaultMessageBox;
    [TabGroup("2","Message Box")] public SoundInfo DefaultTypingSound;
    
    [TabGroup("2","Shop Menu")] public GameObject DefaultShopMenu;
    [TabGroup("2", "Shop Menu")] public SoundInfo DefaultChestSound;
    
    [TabGroup("2","Text Settings")] public Color GoldTextColor;
    [TabGroup("2","Text Settings")] public Color DefaultItemTextColor;
    
    [TabGroup("3","Databases")] public ItemDatabase ItemDatabase;
    [TabGroup("3","Databases")] public CharacterDatabase CharacterDatabase;
    
    [TabGroup("4","Equipment")] 
    public Dictionary<EquippableBase.EquippableTier, int> DefaultEquipmentQualityChance;
    [TabGroup("4", "Equipment")] 
    public Dictionary<EquippableBase.EquippableTier, Color> DefaultEquipmentQualityColor;
}
