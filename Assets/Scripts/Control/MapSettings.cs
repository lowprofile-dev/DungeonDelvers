using System;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

public class MapSettings : SerializedMonoBehaviour
{
    public static MapSettings Instance { get; private set;}

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
        ApplyMapSettings();
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
}

public interface IMapSettingsListener
{
    void ApplyMapSettings(MapSettings mapSettings);
}