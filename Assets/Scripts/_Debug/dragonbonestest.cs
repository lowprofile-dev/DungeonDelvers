using System.Collections;
using DragonBones;
using Sirenix.OdinInspector;
using UnityEngine;

public class dragonbonestest : MonoBehaviour
{
    public GameObject Prefab;
    public RectTransform canvas;
    public UnityArmatureComponent Dragonbones;
    public string animationName;
    public string defaultState;
    
    public void Start()
    {
        
    }

    [Button]
    private void test1()
    {
        var obj = Instantiate(Prefab, canvas);
        obj.SetActive(true);
        Dragonbones = obj.GetComponent<UnityArmatureComponent>();
        Dragonbones.animation.Play(defaultState,0);
    }
    
    [Button]
    private void test2()
    {
        StartCoroutine(PlayAndReturnToDefault());
    }

    private IEnumerator PlayAndReturnToDefault()
    {
        Dragonbones.animation.Play(animationName, 1);
        
        while(Dragonbones.animation.isPlaying)
            yield return new WaitForEndOfFrame();

        Dragonbones.animation.Play(defaultState,0);
        Debug.Log("foi");
    }
}
