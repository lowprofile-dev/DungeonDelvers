using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace DD.Skill.Animation
{
    public abstract class Animation
    {
        public string AnimationName;
        public float SpeedMultiplier = 1f;
        public Vector2 AnimationSize;
        public Vector2 AnimationOffset;
        
        public abstract Task PlaySkillAnimation(Battler source, IEnumerable<Battler> targets);
    
        public void ScaleAnimation(RectTransform rect)
        {
            rect.sizeDelta = AnimationSize;
            rect.localPosition += (Vector3)AnimationOffset;
        }
    }
}

