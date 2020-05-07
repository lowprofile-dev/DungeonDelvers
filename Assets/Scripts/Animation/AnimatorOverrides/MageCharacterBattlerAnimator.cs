using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DD.Animation
{
    public class MageCharacterBattlerAnimator : CharacterBattlerAnimator
    {
        private Element animationElement;

        public Element AnimationElement
        {
            get => animationElement;
            set
            {
                animationElement = value;
                if (EquippedWeaponType == WeaponBase.WeaponType.Shortsword){
                    GameController.Instance.QueueAction(UpdateElement);
                }
            }
        }

        private void UpdateElement()
        {
            var controller = ElementControllers.Find(c => c.Element == animationElement).RuntimeAnimatorController;
            if (controller == Animator.runtimeAnimatorController)
                return;
            Animator.runtimeAnimatorController = controller;
            UpdateAnimator();
        }
    
        public List<ElementController> ElementControllers = new List<ElementController>();

        public struct ElementController
        {
            public Element Element;
            public RuntimeAnimatorController RuntimeAnimatorController;
        }
    }
}

