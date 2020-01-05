using System.Collections.Generic;
using System.Threading.Tasks;
using Sirenix.Utilities;
using SkredUtils;
using UnityEngine;

public class PlayAtTargetSkillAnimation : SkillAnimation
{
    public override async Task PlaySkillAnimation(IBattler source, IEnumerable<IBattler> targets)
    {
        List<Task> Animations = new List<Task>();
        await GameController.Instance.QueueActionAndAwait(() =>
        {
            targets.ForEach(target =>
            {
                var animationObject = GameObject.Instantiate(GameController.Instance.AnimationObjectBase,
                    BattleController.Instance.battleCanvas.transform);

                var animation = animationObject.GetComponent<AnimationObject>();
                animation.transform.position = target.RectTransform.position;
                
                ScaleAnimation(animation.transform as RectTransform);
                
                Animations.Add(animation.PlayAndAwait(AnimationName));
            });
        });

        await Task.WhenAll(Animations);
    }
}