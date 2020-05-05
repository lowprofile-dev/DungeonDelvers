using System.Collections;
using UnityEngine;

[InteractableNode(defaultNodeName = "Play Animation")]
public class PlayAnimationInteraction : Interaction
{
    [Input] public Animator animator;
    [Input] public string animationName;
    
    public override IEnumerator PerformInteraction(Interactable source)
    {
        var anim = GetInputValue<object>("animator", animator) as Animator;
        var animName = GetInputValue("animationName", animationName);
        
        anim.Play(animName);
        
        yield return new WaitForEndOfFrame();
        yield return new WaitWhile(() => anim.GetCurrentAnimatorStateInfo(0).IsName(animName));
    }
}