using System;
using System.Linq;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

#endif

public class SerializableAsset : SerializedScriptableObject
{
    //[PropertyOrder(-999), ReadOnly] public string uniqueIdentifier = "";
    [PropertyOrder(-998)] public bool databaseIgnore = false;
}