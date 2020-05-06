using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sirenix.Utilities;
using UnityEngine;

namespace DD.Skill.Animation
{
    public class FromSourceToTargetSkillAnimation : Animation
    {
        public override async Task PlaySkillAnimation(Battler source, IEnumerable<Battler> targets)
        {
            List<Task> Animations = new List<Task>();
            var leftoverObjects = new List<GameObject>();
            await GameController.Instance.QueueActionAndAwait(() =>
            {
                targets.ForEach(target =>
                {
                    var animationObject = GameObject.Instantiate(GameSettings.Instance.AnimationObject,
                        BattleController.Instance.battleCanvas.transform);

                    leftoverObjects.Add(animationObject);
                    
                    var animation = animationObject.GetComponent<AnimationObject>();
                    animation.transform.position = target.RectTransform.position;

                    ScaleAnimation(animation.transform as RectTransform);
                
                    Animations.Add(
                        GameController.Instance.PlayCoroutine(MoveAnimationCoroutine(source.RectTransform.position,
                            target.RectTransform.position, animationObject.transform, animation.animator, AnimationName), animation));
                });
            });

            await Task.WhenAll(Animations);
            await source.QueueActionAndAwait(() =>
            {
                foreach (var o in leftoverObjects) Object.Destroy(o);
            });
        }

        private IEnumerator MoveAnimationCoroutine(Vector2 source, Vector2 target, Transform animation, Animator animator, string animationName)
        {
            //Funcionando por enquanto. Arrumar a rotação depois (opcional)
            animator.SetFloat("SpeedMultiplier",SpeedMultiplier);
            animator.Play(animationName);
            yield return new WaitForEndOfFrame();

            var info = animator.GetCurrentAnimatorStateInfo(0);
            var elapsedTime = 0f;
            var finishTime = info.length / SpeedMultiplier;

            while (elapsedTime < finishTime)
            {
                elapsedTime += Time.deltaTime;
                animation.position = Vector2.Lerp(source, target, elapsedTime / finishTime);
                yield return null;
            }
            GameObject.Destroy(animation.gameObject);
        }
    }
}
