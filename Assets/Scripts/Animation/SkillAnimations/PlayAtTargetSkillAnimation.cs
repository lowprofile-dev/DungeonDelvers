using System.Collections.Generic;
using System.Threading.Tasks;
using Sirenix.Utilities;
using SkredUtils;
using UnityEngine;

namespace DD.Skill.Animation
{
    public class PlayAtTargetSkillAnimation : Animation
    {
        public override async Task PlaySkillAnimation(Battler source, IEnumerable<Battler> targets)
        {
            List<Task> Animations = new List<Task>();
            await GameController.Instance.QueueActionAndAwait(() =>
            {
                targets.ForEach(target =>
                {
                    var animationObject = GameObject.Instantiate(GameSettings.Instance.AnimationObject,
                        BattleController.Instance.battleCanvas.transform);

                    var animation = animationObject.GetComponent<AnimationObject>();
                    animation.transform.position = target.RectTransform.position;
                
                    ScaleAnimation(animation.transform as RectTransform);
                
                    Animations.Add(animation.PlayAndAwait(AnimationName,SpeedMultiplier));
                });
            });

            await Task.WhenAll(Animations);
        }
    }
}

