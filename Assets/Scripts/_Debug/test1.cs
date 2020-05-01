using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

public class test1 : SerializedMonoBehaviour
{
    public Color color;

    [Button] public void PrintColor()
    {
        Debug.Log(ColorUtility.ToHtmlStringRGB(color));
    }
    
}