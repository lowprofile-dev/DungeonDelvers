using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class MapSettings : SerializedMonoBehaviour
{
    public static MapSettings Instance { get; private set;}

    [FoldoutGroup("Scene Sequence")] public int previousSceneIndex = -1;
    [FoldoutGroup("Scene Sequence")] public int nextSceneIndex = -1;

    [FormerlySerializedAs("Encounters"), FoldoutGroup("Encounter")] public MapEncounterSet DefaultMapEncounterSet = null;
    [FoldoutGroup("Encounter"), ShowInInspector, ReadOnly] private MapEncounterSet _mapEncounterSet;
    public MapEncounterSet MapEncounterSet
    {
        get => _mapEncounterSet;
        set
        {
            if (value == null)
                _mapEncounterSet = DefaultMapEncounterSet;
            else
            {
                _mapEncounterSet = value;
                RollEncounterDistance();
            }
        }
    }
    [FoldoutGroup("Encounter")][ReadOnly] public float RemainingEncounterDistance = 0;
    private Vector2 lastPosition;
    public Color MinimapWallColor;
    public Color MinimapFloorColor;
    
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
        MapEncounterSet = DefaultMapEncounterSet;
        ApplyMapSettings();
    }

    public void FixedUpdate()
    {
        if (MapEncounterSet != null)
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
        var set = MapEncounterSet;
        var distance = Random.Range(set.MinEncounterDistance, set.MaxEncounterDistance);
        RemainingEncounterDistance = distance;
        lastPosition = PlayerController.Instance.transform.position;
    }

    public void StartEncounter()
    {
        var set = MapEncounterSet;
        var encounters = set.MapEncounters;
        var maxChanceValue = encounters.Select(encounter => encounter.Chance).Sum();
        var chosenChanceValue = Random.Range(0, maxChanceValue);

        var chosenIndex = 0;

        for (int i = 0; i < encounters.Count; i++)
        {
            chosenChanceValue -= encounters[i].Chance;
            if (chosenChanceValue <= 0)
            {
                chosenIndex = i;
                break;
            }
        }

        var chosenEncounter = encounters[chosenIndex].Encounter;
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
}

public interface IMapSettingsListener
{
    void ApplyMapSettings(MapSettings mapSettings);
}

public class MapEncounterSet
{
    public List<MapEncounter> MapEncounters = new List<MapEncounter>();
    public float MinEncounterDistance = 10f;
    public float MaxEncounterDistance = 25f;
}

public struct MapEncounter
{
    public GameObject Encounter;
    public int Chance;
}