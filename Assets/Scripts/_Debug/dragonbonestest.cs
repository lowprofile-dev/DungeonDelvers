using System.Collections;
using DragonBones;
using Sirenix.OdinInspector;
using UnityEngine;

public class dragonbonestest : MonoBehaviour
{
    public UnityDragonBonesData DBData;
    public string armatureName;
    public string baseAnimationName;
    public string defaultStateName;
    
    public void Start()
    {
        var go = new GameObject("Aqui");
        go.transform.SetParent(transform);
        var uac = go.AddComponent<UnityArmatureComponent>();
        uac.unityData = DBData;
        uac.armatureName = armatureName;
        uac.armatureBaseName = baseAnimationName;
        uac.animation.Play(defaultStateName, 0);
    }
}
