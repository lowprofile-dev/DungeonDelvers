using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using BrightLib.Animation.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;
using CharacterBattlerAnimation = CharacterBattler.CharacterBattlerAnimation;

namespace DD.Animation
{
    [RequireComponent(typeof(CharacterBattler), typeof(Animator))]
    public class CharacterBattlerAnimator : SerializedMonoBehaviour
    {
        protected CharacterBattler CharacterBattler;
        protected Animator Animator;
        protected WeaponBase.WeaponType? EquippedWeaponType;

        public virtual void LoadControllerForWeapon(WeaponBase.WeaponType? weaponType)
        {
            var battlerAnimator =
                CharacterBattler.Character.Base.BattlerAnimationControllers.First(controller =>
                    controller.WeaponType == weaponType);

            Animator.runtimeAnimatorController = battlerAnimator.AnimatorController;
        }

        protected virtual void Awake()
        {
            Animator = GetComponent<Animator>();
            CharacterBattler = GetComponent<CharacterBattler>();
        }

        protected virtual void Start()
        {
            EquippedWeaponType = (CharacterBattler.Character.Weapon?.EquippableBase as WeaponBase)?.weaponType;
            LoadControllerForWeapon(EquippedWeaponType);
        }

        public IEnumerator PlayAndWait(BattlerAnimationInfo info)
        {
            var hashName = Animator.StringToHash(info.stateName);
            if (!Animator.HasState(0, hashName))
                yield break;
            
            Animator.speed = info.speed;
            
            var playAudio = Animator.GetBehaviours<PlayAudioClip>().FirstOrDefault(pac => pac.identifiableName == info.stateName);
            playAudio?.SetSoundInfo(info.soundInfo);
            
            Animator.Play(hashName);
            
            yield return new WaitForEndOfFrame();
            yield return new WaitWhile(() => Animator.GetCurrentAnimatorStateInfo(0).shortNameHash == hashName);
        }

        public async Task AsyncPlayAndWait(BattlerAnimationInfo info)
        {
            var state = -1;
            await CharacterBattler.QueueActionAndAwait(() =>
            {
                state = Animator.StringToHash(info.stateName);
                if (!Animator.HasState(0, state)) state = -1;
            });
            
            if (state == -1) return;
            
            await CharacterBattler.QueueActionAndAwait(() =>
            {
                Animator.speed = info.speed;
                Animator.Play(state);

                var playAudio = Animator.GetBehaviours<PlayAudioClip>().FirstOrDefault(pac => pac.identifiableName == info.stateName);
                playAudio?.SetSoundInfo(info.soundInfo);
                
            });

            await Task.Delay(5);

            bool condition = false;

            void EvaluteCondition()
            {
                condition = Animator.GetCurrentAnimatorStateInfo(0).shortNameHash == state;
                if (!condition)
                {
                    Animator.speed = 1f;
                }
            }

            await CharacterBattler.QueueActionAndAwait(EvaluteCondition);

            while (condition)
            {
                await CharacterBattler.QueueActionAndAwait(EvaluteCondition);
            }
        }
        
        public IEnumerator PlayAndWait(CharacterBattlerAnimation characterBattlerAnimation, float speedModifier = 1f)
        {
            Animator.speed = speedModifier;
            var state = GetStateNameFromAnimation(characterBattlerAnimation);

            Animator.Play(state);

            yield return new WaitForEndOfFrame();
            yield return new WaitWhile(() => Animator.GetCurrentAnimatorStateInfo(0).IsName(state));
            Animator.speed = 1f;
        }
        
        public async Task AsyncPlayAndWait(CharacterBattlerAnimation characterBattlerAnimation,
            float speedModifier = 1f)
        {
            var state = "";
            await CharacterBattler.QueueActionAndAwait(() =>
                state = GetStateNameFromAnimation(characterBattlerAnimation));

            await CharacterBattler.QueueActionAndAwait(() =>
            {
                Animator.speed = speedModifier;
                Animator.Play(state);
            });

            await Task.Delay(5);

            bool condition = false;

            void EvaluteCondition()
            {
                condition = Animator.GetCurrentAnimatorStateInfo(0).IsName(state);
                if (!condition)
                {
                    Animator.speed = 1f;
                }
            }

            await CharacterBattler.QueueActionAndAwait(EvaluteCondition);

            while (condition)
            {
                await CharacterBattler.QueueActionAndAwait(EvaluteCondition);
            }
        }
        
        public void Play(CharacterBattlerAnimation characterBattlerAnimation, bool lockTransition = false)
        {
            var state = GetStateNameFromAnimation(characterBattlerAnimation);

            if (lockTransition)
            {
                Animator.SetBool("CanTransition", false);
            }

            Animator.Play(state);
        }

        public void Play(string animation, bool lockTransition = false)
        {
            var state = Animator.StringToHash(animation);
            if (!Animator.HasState(0,state))
                return;

            if (lockTransition)
            {
                Animator.SetBool("CanTransition", false);
            }

            Animator.Play(state);
        }

        public virtual void UpdateAnimator()
        {
            //Animator.SetBool("HasWeapon", CharacterBattler.Character.Weapon != null);
            Animator.SetBool("Fainted", CharacterBattler.Fainted);
        }

        public bool CanTransition
        {
            get => Animator.GetBool("CanTransition");
            set => Animator.SetBool("CanTransition", value);
        }

        [Obsolete]
        protected virtual string GetStateNameFromAnimation(CharacterBattlerAnimation characterBattlerAnimation)
        {
            switch (characterBattlerAnimation)
            {
                case CharacterBattlerAnimation.Attack:
                case CharacterBattlerAnimation.Idle:
                case CharacterBattlerAnimation.Damage:
                case CharacterBattlerAnimation.Cast:
                case CharacterBattlerAnimation.Fainted:
                default:
                    return characterBattlerAnimation.ToString();
            }
        }
    }
}