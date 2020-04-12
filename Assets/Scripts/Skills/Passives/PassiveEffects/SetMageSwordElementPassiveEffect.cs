using UnityEngine;

public class SetMageSwordElementPassiveEffect : PassiveEffect, IOnApplyPassiveEffect
{
    public Element Element;
    
    public void OnApply(Battler battler)
    {
        var mageAnimator = (battler as CharacterBattler).Animator as MageCharacterBattlerAnimator;
        mageAnimator.AnimationElement = Element;
    }

    public void OnUnapply(Battler battler)
    {
        var mageAnimator = (battler as CharacterBattler).Animator as MageCharacterBattlerAnimator;
        mageAnimator.AnimationElement = Element.None;
    }
}