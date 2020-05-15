using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class testprefabvar : MonoBehaviour
{
    public testprefabvar reference;
    public string text;
    
    private void Start()
    {
        Debug.Log(reference.text);
    }
}
