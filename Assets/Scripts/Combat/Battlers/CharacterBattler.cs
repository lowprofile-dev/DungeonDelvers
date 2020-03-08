using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using SkredUtils;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Object = System.Object;
using Random = System.Random;

// ReSharper disable RedundantAssignment

public class CharacterBattler : Battler
{
    public CharacterBattlerAnimator Animator;
    public Character Character;
    private Image image;

    #region Control

    public Dictionary<object, object> BattleDictionary { get; private set; }

    private void Awake()
    {
        image = GetComponent<Image>();
        Animator = GetComponent<CharacterBattlerAnimator>();
    }

    public void Create(Character character)
    {
        BattleDictionary = new Dictionary<object, object>();
        Character = character;
        BattlerName = Character.Base.CharacterName;
        Level = PlayerController.Instance.PartyLevel;
        Stats = character.Stats;
        CurrentHp = character.CurrentHp;
        CurrentEp = Stats.InitialEp;
        Skills = character.Skills;
        Passives = character.Passives;
        StatusEffectInstances = new List<StatusEffectInstance>();
    }

    public void CommitChanges()
    {
        Character.CurrentHp = currentHp;
    }

    private void SetHighestHp()
    {
        var hasPreviousHighestHp = BattleDictionary.TryGetValue("HighestHP", out var highestHpObject);

        if (hasPreviousHighestHp)
        {
            var highestHp = (int) highestHpObject;
            if (CurrentHp > highestHp)
            {
                Debug.Log($"{BattlerName} Highest HP: {CurrentHp}");
                BattleDictionary["HighestHP"] = CurrentHp;
            }
        }
        else
        {
            Debug.Log($"{BattlerName} Highest HP: {CurrentHp}");
            BattleDictionary["HighestHP"] = CurrentHp;
        }
    }

    #endregion

    #region Stats

    [FoldoutGroup("Stats"), ShowInInspector, Sirenix.OdinInspector.ReadOnly]
    private int currentEp;

    public override int CurrentEp
    {
        get => currentEp;
        set
        {
            currentEp = value;
            currentEp = Mathf.Clamp(currentEp, 0, 100);
        }
    }

    [FoldoutGroup("Stats"), ShowInInspector, Sirenix.OdinInspector.ReadOnly]
    private int currentHp;

    public override int CurrentHp
    {
        get => currentHp;
        set
        {
            currentHp = value;
            currentHp = Mathf.Clamp(currentHp, 0, Stats.MaxHp);
            UpdateAnimator();
            SetHighestHp();
        }
    }

    [FoldoutGroup("Skills")] public List<PlayerSkill> Skills { get; private set; }

    [FoldoutGroup("Status Effects"), ShowInInspector, Sirenix.OdinInspector.ReadOnly]
    public override List<StatusEffectInstance> StatusEffectInstances { get; set; }

    #endregion

    #region Animation

    public IEnumerator PlayAndWait(CharacterBattlerAnimation characterBattlerAnimation) =>
        Animator.PlayAndWait(characterBattlerAnimation);

    public Task AsyncPlayAndWait(CharacterBattlerAnimation animation) =>
        Animator.AsyncPlayAndWait(animation);

    public void Play(CharacterBattlerAnimation animation, bool lockTransition = false) =>
        Animator.Play(animation, lockTransition);

    public void UpdateAnimator() => Animator.UpdateAnimator();

    public bool CanTransition => Animator.CanTransition;

    public enum CharacterBattlerAnimation
    {
        Idle,
        Attack,
        Damage,
        Cast,
        Fainted
    } 

    #endregion
    
    #region TurnEvents
    
    public override async Task<Turn> GetTurn()
    {
        Debug.Log($"Fazendo o turno de {Character.Base.CharacterName}");

        if (Fainted)
            return null;

        var turn = await BattleController.Instance.battleCanvas.GetTurn(this);

        return turn.Skill == null ? null : turn;
    }

    protected override async Task AnimateTurn(Turn turn)
    {
        var skill = turn.Skill;
        var playerSkill = skill as PlayerSkill;
        await AsyncPlayAndWait(playerSkill.AnimationType);

        if (skill.SkillAnimation != null)
        {
            await skill.SkillAnimation.PlaySkillAnimation(this, turn.Targets);
        }
    }

    protected override async Task AnimateEffectResult(EffectResult effectResult)
    {
        switch (effectResult)
        {
            case DamageEffect.DamageEffectResult damageEffectResult:
            {
                var tasks = new List<Task>();
                if (damageEffectResult.DamageDealt > 0)
                {
                    tasks.Add(AsyncPlayAndWait(CharacterBattlerAnimation.Damage));
                    tasks.Add(PlayCoroutine(DamageBlinkCoroutine()));
                }
                
                var time = effectResult.skillInfo.HasCrit ? 1.4f : 1f;

                tasks.Add(BattleController.Instance.battleCanvas.ShowSkillResult(this, damageEffectResult.DamageDealt.ToString(), Color.white, time));
                
                await Task.WhenAll(tasks);
                break;
            }
            case HealEffect.HealEffectResult healEffectResult:
                await BattleController.Instance.battleCanvas.ShowSkillResult(this, healEffectResult.AmountHealed.ToString(),
                    Color.green);
                break;
            case GainApEffect.GainApEffectResult gainApEffectResult:
            {
                await BattleController.Instance.battleCanvas.ShowSkillResult(this, gainApEffectResult.ApGained.ToString(),
                    Color.cyan);
                break;
            }
            case MultiHitDamageEffect.MultiHitDamageEffectResult multiHitDamageEffectResult:
            {
                //Botar um pequeno delay entre cada hit do dano graficamente depois
                var tasks = new List<Task>();
                if (multiHitDamageEffectResult.TotalDamageDealt > 0)
                {
                    tasks.Add(AsyncPlayAndWait(CharacterBattlerAnimation.Damage));
                    tasks.Add(PlayCoroutine(DamageBlinkCoroutine()));
                }

                var damageString = string.Join("\n", multiHitDamageEffectResult.HitResults.Select(result => result.DamageDealt.ToString()));
                tasks.Add(BattleController.Instance.battleCanvas.ShowSkillResult(this, damageString, Color.white));
                
                await Task.WhenAll(tasks);
                break;
            }
        }
    }

    #endregion
    
    #region Animations
    private IEnumerator DamageBlinkCoroutine()
    {
        var normalColor = image.color;
        var blinkingColor = new Color(image.color.r, image.color.g, image.color.grayscale, 0.3f);

        for (int i = 0; i < 5; i++)
        {
            image.color = blinkingColor;
            yield return new WaitForSeconds(0.05f);
            image.color = normalColor;
            yield return new WaitForSeconds(0.05f);
        }
    }
    #endregion

    #region Methods

    public PlayerSkill[] AvailableSkills => Skills.Where(skill => skill.HasRequiredWeapon(Character)).ToArray();

    #endregion
    
    public RectTransform RectTransform => transform as RectTransform;
}

