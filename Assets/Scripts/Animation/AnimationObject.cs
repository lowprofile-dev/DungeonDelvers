using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AnimationObject : MonoBehaviour
{
    public Animator animator;
    
    public async Task PlayAndAwait(string animation)
    {
        await GameController.Instance.QueueActionAndAwait(() =>
        {
            animator.Play(animation);
        });
        
        await Task.Delay(5);

        bool? condition = null;

        Action evaluateCondition = () =>
        {
            condition = animator.GetCurrentAnimatorStateInfo(0).IsName(animation.ToString());
        };

        await GameController.Instance.QueueActionAndAwait(evaluateCondition);
        
        while (condition.HasValue && condition.Value == true)
        {
            await GameController.Instance.QueueActionAndAwait(evaluateCondition);
        }
    }
}
