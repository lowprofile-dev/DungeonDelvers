using System.Collections;
using UnityEngine;

[InteractableNode(defaultNodeName = "Play Animation")]
public class PlayAnimationInteraction : Interaction
{
    [Input] public Animator animator;
    [Input] public string animationName;
    
    public override IEnumerator PerformInteraction(Interactable source)
    {
        var anim = GetInputValue("animator", animator);
        var animName = GetInputValue("animationName", animationName);
        
        yield return null;
        if (anim.GetCurrentAnimatorStateInfo(0).IsName(animName))
            yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length + anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
    }
}