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
    public Animator animator;
    public Character Character;
    private Image image;

    #region Control
    public Dictionary<object, object> BattleDictionary { get; private set; }
    
    private void Start()
    {
        image = GetComponent<Image>();
    }

    public void Create(Character character)
    {
        BattleDictionary = new Dictionary<object, object>();
        Character = character;
        Stats = character.Stats;
        CurrentHp = character.CurrentHp;
        CurrentEp = Stats.InitialEp;
        Skills = character.Skills;
        Passives = character.Passives;
        StatusEffects = new List<StatusEffect>();
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
                Debug.Log($"{Name} Highest HP: {CurrentHp}");
                BattleDictionary["HighestHP"] = CurrentHp;
            }
        }
        else
        {
            Debug.Log($"{Name} Highest HP: {CurrentHp}");
            BattleDictionary["HighestHP"] = CurrentHp;
        }
    }
    #endregion
    
    #region Stats
    public int Level => PlayerController.Instance.PartyLevel;
    public string Name => Character.Base.CharacterName;
    [FoldoutGroup("Stats")] public Stats Stats { get; private set; }
    [FoldoutGroup("Stats"), ShowInInspector, Sirenix.OdinInspector.ReadOnly] private int currentEp;
    public int CurrentEp
    {
        get => currentEp;
        set
        {
            currentEp = value;
            currentEp = Mathf.Clamp(currentEp, 0, 100);
        }
    }
    [FoldoutGroup("Stats"), ShowInInspector, Sirenix.OdinInspector.ReadOnly] private int currentHp;
    public int CurrentHp
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
    public bool Fainted => CurrentHp == 0;
    [FoldoutGroup("Skills")] public List<PlayerSkill> Skills { get; private set; }
    [FoldoutGroup("Passives"), ShowInInspector, Sirenix.OdinInspector.ReadOnly] public List<Passive> Passives { get; set; } = new List<Passive>();
    [FoldoutGroup("Status Effects"), ShowInInspector, Sirenix.OdinInspector.ReadOnly] public List<StatusEffect> StatusEffects
    {
        get;
        set;
    }
    #endregion

    #region Animation

    public IEnumerator PlayAndWait(CharacterBattlerAnimation animation)
    {
        animator.Play(animation.ToString());

        yield return new WaitForEndOfFrame();
        yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).IsName(animation.ToString()));
    }

    public async Task AsyncPlayAndWait(CharacterBattlerAnimation animation)
    {
        await QueueActionAndAwait(() =>
        {
            Play(animation);
        });
        
        await Task.Delay(5);

        bool? condition = null;

        Action evaluateCondition = () =>
        {
            condition = animator.GetCurrentAnimatorStateInfo(0).IsName(animation.ToString());
        };

        await QueueActionAndAwait(evaluateCondition);
        
        while (condition.HasValue && condition.Value == true)
        {
            await QueueActionAndAwait(evaluateCondition);
        }
    }

    public void Play(CharacterBattlerAnimation animation, bool lockTransition = false)
    {
        if (lockTransition)
        {
            animator.SetBool("CanTransition",false);
        }
        animator.Play(animation.ToString());
    }

    public void UpdateAnimator()
    {
        animator.SetBool("HasWeapon", Character.Weapon != null);
        animator.SetBool("Fainted", Fainted);
    }

    public bool CanTransition
    {
        get => animator.GetBool("CanTransition");
        set => animator.SetBool("CanTransition", value);
    }

    public enum CharacterBattlerAnimation
    {
        Idle,
        IdleNoWeapon,
        Attack,
        Damage,
        Cast,
        Fainted
    } 

    #endregion
    
    #region TurnEvents
//    public async Task TurnStart()
//    {
//        currentEp += Stats.EpGain;
//        Debug.Log($"Começou turno de {Character.Base.CharacterName}");
//
//        var expiredStatusEffects = StatusEffects.Where(statusEffect => statusEffect.TurnDuration <= BattleController.Instance.CurrentTurn);
//
//        expiredStatusEffects.ForEach(expired => StatusEffects.Remove(expired));
//        
//        var turnStartPassives =
//            Passives.SelectMany(passive => passive.Effects.Where(effect => effect is ITurnStartPassiveEffect))
//                .Concat(
//                    StatusEffects.SelectMany(statusEffect => statusEffect.Effects.Where(effect => effect is ITurnStartPassiveEffect)))
//                .OrderByDescending(effect => effect.Priority).Cast<ITurnStartPassiveEffect>();
//
//        foreach (var turnStartPassive in turnStartPassives)
//        {
//            await turnStartPassive.OnTurnStart(this);
//        }
//    }

//    public async Task TurnEnd()
//    {
//        //Fazer efeitos do que precisar aqui, quando precisar
//        Debug.Log($"Acabou o turno de {Character.Base.CharacterName}");
//    }

    public override async Task<Turn> GetTurn()
    {
        Debug.Log($"Fazendo o turno de {Character.Base.CharacterName}");

        if (Fainted)
            return null;

        var turn = await BattleController.Instance.battleCanvas.GetTurn(this);

        return turn.Skill == null ? null : turn;
    }

//    public async Task ExecuteTurn(Turn turn)
//    {
//        var skill = turn.Skill;
//        
//        Debug.Log($"Executando o turno de {Character.Base.CharacterName}");
//
//        if (skill.EpCost > CurrentEp)
//        {
//            Debug.LogError($"{Character.Base.CharacterName} tentou usar uma skill com custo maior que o EP Atual");
//            return;
//        }
//
//        CurrentEp -= skill.EpCost;
//        
//        QueueAction(() =>
//        {
//            BattleController.Instance.battleCanvas.battleInfoPanel.ShowInfo(skill.SkillName);
//        });
//        
//        var playerSkill = skill as PlayerSkill;
//        await AsyncPlayAndWait(playerSkill.AnimationType);
//
//        if (skill.SkillAnimation != null)
//        {
//            await skill.SkillAnimation.PlaySkillAnimation(this, turn.Targets);
//        }
//        
//        QueueAction(() =>
//        {
//            BattleController.Instance.battleCanvas.battleInfoPanel.HideInfo();
//        });
//    }

//    public async Task<IEnumerable<EffectResult>> ReceiveSkill(Battler source, Skill skill)
//    {
//        Debug.Log($"Recebendo Skill em {Character.Base.CharacterName}");
//
//        bool hasHit;
//
//        if (skill.TrueHit)
//        {
//            Debug.Log($"{source.Name} acertou {Name} com {skill.SkillName} por ter True Hit");
//            hasHit = true;
//        }
//        else
//        {
//            var accuracy = source.Stats.Accuracy + skill.AccuracyModifier;
//            var evasion = Stats.Evasion;
//
//            var hitChance = accuracy - evasion;
//
//            var rng = GameController.Instance.Random.NextDouble();
//
//            hasHit = rng <= hitChance;
//            
//            Debug.Log($"{source.Name} {(hasHit ? "acertou":"errou")} {Name} com {skill.SkillName} -- Acc: {accuracy}, Eva: {evasion}, Rng: {rng}");
//        }
//        if (hasHit)
//        {
//            var results = new List<EffectResult>();
//
//            bool hasCrit;
//
//            if (!skill.CanCritical)
//            {
//                hasCrit = false;
//                Debug.Log($"{source.Name} não critou {Name} com {skill.SkillName} por não poder critar.");
//            }
//            else
//            {
//                var critAccuracy = source.Stats.CritChance + skill.CriticalModifier;
//                var critEvasion = Stats.CritAvoid;
//
//                var critChance = critAccuracy - critEvasion;
//
//                var rng = GameController.Instance.Random.NextDouble();
//
//                hasCrit = rng <= critChance;
//                
//                Debug.Log($"{source.Name} {(hasCrit ? "":"não")} critou {Name} com {skill.SkillName} -- Acc: {critAccuracy}, Eva: {critEvasion}, Rng: {rng}");
//            }
//            
//            var skillInfo = new SkillInfo
//            {
//                HasCrit = hasCrit,
//                Skill = skill,
//                Source = source,
//                Target = this
//            };
//
//            var effects = hasCrit ? skill.CriticalEffects : skill.Effects;
//            
//            foreach (var effect in effects)
//            {
//                results.Add(
//                    await ReceiveEffect(new EffectInfo
//                    {
//                        SkillInfo = skillInfo,
//                        Effect = effect
//                    }));
//            }
//            return results;
//        }
//        else
//        {
//            await BattleController.Instance.battleCanvas.ShowSkillResult(this, "Miss!", Color.white);
//            return new EffectResult[] { };
//            //retonar missresult depois(?)
//        }
//    }

//    public async Task<EffectResult> ReceiveEffect(EffectInfo effectInfo)
//    {
//        EffectResult effectResult = null;
//
//        await QueueActionAndAwait(() =>
//        {
//            effectResult = effectInfo.Effect.ExecuteEffect(effectInfo.SkillInfo);
//        });
//
//        switch (effectResult)
//        {
//            case DamageEffect.DamageEffectResult damageEffectResult:
//            {
//                var tasks = new List<Task>();
//                if (damageEffectResult.DamageDealt > 0)
//                {
//                    tasks.Add(AsyncPlayAndWait(CharacterBattlerAnimation.Damage));
//                    tasks.Add(PlayCoroutine(DamageBlinkCoroutine()));
//                }
//                
//                tasks.Add(BattleController.Instance.battleCanvas.ShowSkillResult(this, damageEffectResult.DamageDealt.ToString(), Color.white));
//                
//                await Task.WhenAll(tasks);
//                break;
//            }
//            case HealEffect.HealEffectResult healEffectResult:
//                await BattleController.Instance.battleCanvas.ShowSkillResult(this, healEffectResult.AmountHealed.ToString(),
//                    Color.green);
//                break;
//            case GainApEffect.GainApEffectResult gainApEffectResult:
//            {
//                await BattleController.Instance.battleCanvas.ShowSkillResult(this, gainApEffectResult.ApGained.ToString(),
//                    Color.cyan);
//                break;
//            }
//            case MultiHitDamageEffect.MultiHitDamageEffectResult multiHitDamageEffectResult:
//            {
//                //Botar um pequeno delay entre cada hit do dano graficamente depois
//                var tasks = new List<Task>();
//                if (multiHitDamageEffectResult.TotalDamageDealt > 0)
//                {
//                    tasks.Add(AsyncPlayAndWait(CharacterBattlerAnimation.Damage));
//                    tasks.Add(PlayCoroutine(DamageBlinkCoroutine()));
//                }
//
//                var damageString = string.Join("\n", multiHitDamageEffectResult.HitResults.Select(result => result.DamageDealt.ToString()));
//                tasks.Add(BattleController.Instance.battleCanvas.ShowSkillResult(this, damageString, Color.white));
//                
//                await Task.WhenAll(tasks);
//                break;
//            }
//        }
//
//        return effectResult;
//    }

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
                
                tasks.Add(BattleController.Instance.battleCanvas.ShowSkillResult(this, damageEffectResult.DamageDealt.ToString(), Color.white));
                
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

