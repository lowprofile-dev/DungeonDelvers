using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public abstract class SkillAnimation
{
    public string AnimationName;
    public float SpeedMultiplier = 1f;
    public Vector2 AnimationSize;
    public abstract Task PlaySkillAnimation(IBattler source, IEnumerable<IBattler> targets);

    public void ScaleAnimation(RectTransform rect)
    {
        rect.sizeDelta = AnimationSize;
    }
    
    // public enum SkillAnimationScaling
    // {
    //     Constant,
    //     ScaleWithTarget,
    //     ScaleWithSource
    // }
}