using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DD.Animation;
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
    protected override void Awake()
    {
        base.Awake();
        image = GetComponent<Image>();
        Animator = GetComponent<CharacterBattlerAnimator>();
    }

    public void Create(Character character)
    {
        BattleDictionary = new Dictionary<object, object>();
        Character = character;
        BattlerName = Character.Base.CharacterName;
        Level = PlayerController.Instance.PartyLevel;
        BaseStats = character.BaseStats;
        BonusStats = character.BonusStats;
        CurrentHp = character.CurrentHp;
        CurrentEp = Stats.InitialEp;
        Skills = character.Skills;
        Passives = character.Passives;
        StatusEffectInstances = new List<StatusEffectInstance>();
        HitSound = character.Base.HitSound;
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
            currentEp = Mathf.Clamp(currentEp, 0, 200);
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
            SetHighestHp();
            UpdateAnimator();
        }
    }

    [FoldoutGroup("Skills")] public List<PlayerSkill> Skills { get; private set; }

    [FoldoutGroup("Status Effects"), ShowInInspector, Sirenix.OdinInspector.ReadOnly]
    public override List<StatusEffectInstance> StatusEffectInstances { get; set; }

    #endregion

    #region Animation

    public IEnumerator PlayAndWait(CharacterBattlerAnimation characterBattlerAnimation, float speed = 1f) =>
        Animator.PlayAndWait(characterBattlerAnimation,speed);
    
    public Task AsyncPlayAndWait(CharacterBattlerAnimation animation, float speed = 1f) =>
        Animator.AsyncPlayAndWait(animation, speed);
    
    public void Play(CharacterBattlerAnimation animation, bool lockTransition = false) =>
        Animator.Play(animation, lockTransition);

    public IEnumerator PlayAndWait(BattlerAnimationInfo info) => Animator.PlayAndWait(info);

    public Task AsyncPlayAndWait(BattlerAnimationInfo info) => Animator.AsyncPlayAndWait(info);

    public void Play(string animation, bool lockTransition = false) => Animator.Play(animation, lockTransition);

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
        var playerSkill = (PlayerSkill)skill;

        foreach (var animation in playerSkill.Animations)
        {
            await animation.Play(this);
        }

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

                tasks.Add(BattleController.Instance.battleCanvas.ShowSkillResultAsync(this, damageEffectResult.DamageDealt.ToString(), Color.white, time));
                
                await Task.WhenAll(tasks);
                break;
            }
            case HealEffect.HealEffectResult healEffectResult:
                await BattleController.Instance.battleCanvas.ShowSkillResultAsync(this, healEffectResult.AmountHealed.ToString(),
                    Color.green);
                break;
            case GainApEffect.GainApEffectResult gainApEffectResult:
            {
                await BattleController.Instance.battleCanvas.ShowSkillResultAsync(this, gainApEffectResult.ApGained.ToString(),
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
                tasks.Add(BattleController.Instance.battleCanvas.ShowSkillResultAsync(this, damageString, Color.white));
                
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
}

