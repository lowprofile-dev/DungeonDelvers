using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PlayAnimationInteraction : Interaction
{
    public Animator animator;
    public string animationName;

    public override void Run(Interactable source)
    {
        animator.Play(animationName);
    }

    public override IEnumerator Completion
    {
        get
        {
            yield return null;
            if (animator.GetCurrentAnimatorStateInfo(0).IsName(animationName))
                yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length + animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
        }
    }

}

