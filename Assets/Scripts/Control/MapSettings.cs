using System;
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
        
        Instance
    }

    public Color OverrideBackgroundColor;
    //encontros e tal
}