using System.Collections.Generic;
using System.Threading.Tasks;
using Sirenix.Utilities;
using SkredUtils;
using UnityEngine;

namespace DD.Skill.Animation
{
    public class PlayAtTargetSkillAnimation : Animation
    {
        public SoundInfo SoundInfo = new SoundInfo();
        
        public override async Task PlaySkillAnimation(Battler source, IEnumerable<Battler> targets)
        {
            List<Task> Animations = new List<Task>();
            var leftoverObjects = new List<GameObject>();
            await source.QueueActionAndAwait(() =>
            {
                targets.ForEach(target =>
                {
                    var animationObject = GameObject.Instantiate(GameSettings.Instance.AnimationObject,
                        BattleController.Instance.battleCanvas.transform);

                    leftoverObjects.Add(animationObject);
                    
                    var animation = animationObject.GetComponent<AnimationObject>();
                    animation.transform.position = target.RectTransform.position;
                
                    ScaleAnimation(animation.transform as RectTransform);
                
                    Animations.Add(animation.PlayAndAwait(AnimationName,SoundInfo,SpeedMultiplier));
                });
            });

            await Task.WhenAll(Animations);
            await source.QueueActionAndAwait(() =>
            {
                foreach (var o in leftoverObjects) Object.Destroy(o);
            });
        }
    }
}

