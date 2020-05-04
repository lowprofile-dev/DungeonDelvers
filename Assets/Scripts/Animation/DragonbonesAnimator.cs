using System;
using System.Collections;
using DragonBones;
using SkredUtils;
using UnityEngine;

public class DragonbonesAnimator : MonoBehaviour
{
    public UnityArmatureComponent Dragonbones;
    public string defaultAnimationName;

    private void Start()
    {
        this.Ensure(ref Dragonbones);
        Dragonbones.animation.Play(defaultAnimationName, 0);
    }

    private IEnumerator PlayAnimation(string animationName)
    {
        if (!Dragonbones.animation.animations.ContainsKey(animationName))
        {
            Debug.LogError($"Dragonbones does not contain animation {animationName}.");
            yield break;
        }

        Dragonbones.animation.Play(animationName, 1);
        
        while (Dragonbones.animation.isPlaying)
            yield return new WaitForEndOfFrame();

        Dragonbones.animation.Play(defaultAnimationName, 0);
    }
}
