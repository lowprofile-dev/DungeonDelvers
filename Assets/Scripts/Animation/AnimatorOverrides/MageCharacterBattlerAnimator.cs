using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MageCharacterBattlerAnimator : CharacterBattlerAnimator
{
    private Element animationElement;

    public Element AnimationElement
    {
        get => animationElement;
        set
        {
            animationElement = value;
            if (EquippedWeaponType == WeaponBase.WeaponType.Sword1H){
                var controller = ElementControllers.Find(c => c.Element == value).RuntimeAnimatorController;
                Animator.runtimeAnimatorController = controller;
                UpdateAnimator();
            }
        }
    }
    
    public List<ElementController> ElementControllers = new List<ElementController>();

    public struct ElementController
    {
        public Element Element;
        public RuntimeAnimatorController RuntimeAnimatorController;
    }
}
