using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class MapSettings : SerializedMonoBehaviour
{
    public static MapSettings Instance { get; private set;}

    [TabGroup("Scene Sequence")] public int previousSceneIndex = -1;
    [TabGroup("Scene Sequence")] public int nextSceneIndex = -1;
    
    [TabGroup("Encounter")]public bool EncounterEnabled = true;
    [TabGroup("Encounter")]public float MinEncounterDistance = 10f;
    [TabGroup("Encounter")]public float MaxEncounterDistance = 25f;
    [TabGroup("Encounter")]public List<MapEncounter> Encounters = new List<MapEncounter>();
    [TabGroup("Encounter")][ReadOnly] public float RemainingEncounterDistance = 0;
    private Vector2 lastPosition;
    
    public void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(this);
            return;
        }
        
        Instance = this;
    }

    public void Start()
    {
        RollEncounterDistance();
        ApplyMapSettings();
    }

    public void FixedUpdate()
    {
        if (EncounterEnabled)
        {
            var currentPosition = (Vector2)PlayerController.Instance.transform.position;
            var deltaDistance = (currentPosition-lastPosition).magnitude;
            if (deltaDistance < 10) //Threshold
            {
                RemainingEncounterDistance -= deltaDistance;
                if (RemainingEncounterDistance <= 0)
                {
                    StartEncounter();
                    RollEncounterDistance();
                }
            }

            lastPosition = currentPosition;
        }
    }

    private void RollEncounterDistance()
    {
        var distance = Random.Range(MinEncounterDistance, MaxEncounterDistance);
        RemainingEncounterDistance = distance;
        lastPosition = PlayerController.Instance.transform.position;
    }

    public void StartEncounter()
    {
        if (!Encounters.Any())
        {
            EncounterEnabled = false;
            return;
        }
        
        var maxChanceValue = Encounters.Select(encounter => encounter.Chance).Sum();
        var chosenChanceValue = Random.Range(0, maxChanceValue);

        var chosenIndex = 0;

        for (int i = 0; i < Encounters.Count; i++)
        {
            chosenChanceValue -= Encounters[i].Chance;
            if (chosenChanceValue <= 0)
            {
                chosenIndex = i;
                break;
            }
        }

        var chosenEncounter = Encounters[chosenIndex].Encounter;
        BattleController.Instance.BeginBattle(chosenEncounter);
    }
    
    public Color OverrideBackgroundColor;
    
    public void ApplyMapSettings()
    {
        var listeners = Object.FindObjectsOfType<MonoBehaviour>()
            .Where(monoBehaviour => monoBehaviour is IMapSettingsListener)
            .Cast<IMapSettingsListener>();

        foreach (var listener in listeners)
        {
            listener.ApplyMapSettings(this);
        }
    }
    //encontros e tal
    
    #if UNITY_EDITOR

    [ShowInInspector] public TransitionPoint mapStart => TransitionPoint.StartPoint;
    [ShowInInspector] public TransitionPoint mapEnd => TransitionPoint.EndPoint;

    #endif
}

public interface IMapSettingsListener
{
    void ApplyMapSettings(MapSettings mapSettings);
}

public struct MapEncounter
{
    public GameObject Encounter;
    public int Chance;
}