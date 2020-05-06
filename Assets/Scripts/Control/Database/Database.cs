using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Database<T> : SerializedScriptableObject where T : SerializableAsset
{
    public string databaseVersion = "";
    public Dictionary<int, T> Content = new Dictionary<int, T>();

    private Dictionary<T, int> Reversed = new Dictionary<T, int>();
    
    public int? GetId(T t)
    {
        foreach (var kvp in Content)
        {
            if (kvp.Value == t) return kvp.Key;
        }
        return null;
    }

#if UNITY_EDITOR
    private void Reset()
    {
        databaseVersion = Application.version;
    }
    
    [Button]
    private void BuildDatabase()
    {
        Content = new Dictionary<int, T>();
        var resources = Resources.LoadAll<T>("");
        int index = 0;
        foreach (var resource in resources)
        {
            if (resource.databaseIgnore == false) Content.Add(index++,resource);
        }
    }
#endif
}
