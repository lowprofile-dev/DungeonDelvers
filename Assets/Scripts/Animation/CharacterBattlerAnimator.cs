using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using CharacterBattlerAnimation = CharacterBattler.CharacterBattlerAnimation;

[RequireComponent(typeof(CharacterBattler), typeof(Animator))]
public class CharacterBattlerAnimator : SerializedMonoBehaviour
{
    public Dictionary<string, int> AnimatorValues = new Dictionary<string, int>();
    protected CharacterBattler CharacterBattler;
    protected Animator Animator;

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
        var weaponType = (CharacterBattler.Character.Weapon?.EquippableBase as WeaponBase)?.weaponType;
        LoadControllerForWeapon(weaponType);
    }

    public IEnumerator PlayAndWait(CharacterBattlerAnimation characterBattlerAnimation)
    {
        var state = GetStateNameFromAnimation(characterBattlerAnimation);

        Animator.Play(state);
        
        yield return new WaitForEndOfFrame();
        yield return new WaitWhile(() => Animator.GetCurrentAnimatorStateInfo(0).IsName(state));
    }

    public async Task AsyncPlayAndWait(CharacterBattlerAnimation characterBattlerAnimation)
    {
        var state = "";
        await CharacterBattler.QueueActionAndAwait(() => state = GetStateNameFromAnimation(characterBattlerAnimation)); 

        await CharacterBattler.QueueActionAndAwait(() =>
        {
            Debug.Log(state);
            Animator.Play(state);
        });

        await Task.Delay(5);

        bool condition = false;
        
        void EvaluteCondition()
        {
            condition = Animator.GetCurrentAnimatorStateInfo(0).IsName(state);
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