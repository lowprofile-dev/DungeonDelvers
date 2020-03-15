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
    
//    protected override string GetStateNameFromAnimation(CharacterBattler.CharacterBattlerAnimation characterBattlerAnimation)
//    {
//        if (characterBattlerAnimation == CharacterBattler.CharacterBattlerAnimation.Attack || characterBattlerAnimation == CharacterBattler.CharacterBattlerAnimation.Idle)
//        {
//            var element = Animator.GetInteger("MageSwordElement");
//            var baseCommand = base.GetStateNameFromAnimation(characterBattlerAnimation);
//
//            switch (element)
//            {
//                case 0:
//                    return baseCommand;
//                case 1:
//                    return $"{baseCommand}_Fire";
//                case 2:
//                    return $"{baseCommand})_Water";
//                case 3:
//                    return $"{baseCommand}_Earth";
//            }
//        }
//    
//        return base.GetStateNameFromAnimation(characterBattlerAnimation);
//    }
}
