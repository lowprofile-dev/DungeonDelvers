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

// ReSharper disable RedundantAssignment

public class CharacterBattler : AsyncMonoBehaviour, IBattler
{
    public Animator animator;
    public Character Character;
    private Image image;

    #region Control
    public Dictionary<string, object> BattleDictionary { get; private set; }
    
    private void Start()
    {
        image = GetComponent<Image>();
    }

    public void Create(Character character)
    {
        Character = character;
        Stats = character.Stats;
        CurrentHp = character.CurrentHp;
        CurrentEp = Stats.InitialEp;
        Skills = character.Skills;
        Passives = character.Passives;
        BattleDictionary = new Dictionary<string, object>();
    }
    
    public void CommitChanges()
    {
        Character.CurrentHp = currentHp;
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
        }
    }
    public bool Fainted => CurrentHp == 0;
    [FoldoutGroup("Skills")] public List<PlayerSkill> Skills { get; private set; }
    [FoldoutGroup("Passives"), ShowInInspector, Sirenix.OdinInspector.ReadOnly] public List<Passive> Passives { get; set; } = new List<Passive>();

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
    public async Task TurnStart()
    {
        currentEp += Stats.EpGain;
        Debug.Log($"Começou turno de {Character.Base.CharacterName}");

        var turnStartPassives =
            Passives.SelectMany(passive => passive.Effects.Where(effect => effect is ITurnStartPassiveEffect))
                .OrderByDescending(effect => effect.Priority).Cast<ITurnStartPassiveEffect>();

        foreach (var turnStartPassive in turnStartPassives)
        {
            await turnStartPassive.OnTurnStart(this);
        }
    }

    public async Task TurnEnd()
    {
        //Fazer efeitos do que precisar aqui, quando precisar
        Debug.Log($"Acabou o turno de {Character.Base.CharacterName}");
    }

    public async Task<Turn> GetTurn()
    {
        Debug.Log($"Fazendo o turno de {Character.Base.CharacterName}");

        if (Fainted)
            return new Turn()
            {
                Skill = null
            };

        var turn = await BattleController.Instance.battleCanvas.GetTurn(this);

        return turn;
    }

    public async Task ExecuteTurn(IBattler source, Skill skill, IEnumerable<IBattler> targets)
    {
        Debug.Log($"Executando o turno de {Character.Base.CharacterName}");

        if (skill.EpCost > CurrentEp)
        {
            Debug.LogError($"{Character.Base.CharacterName} tentou usar uma skill com custo maior que o EP Atual");
            return;
        }

        CurrentEp -= skill.EpCost;
        
        QueueAction(() =>
        {
            BattleController.Instance.battleCanvas.battleInfoPanel.ShowInfo(skill.SkillName);
        });
        
        var playerSkill = skill as PlayerSkill;
        await AsyncPlayAndWait(playerSkill.AnimationType);

        if (skill.SkillAnimation != null)
        {
            await skill.SkillAnimation.PlaySkillAnimation(source, targets);
        }
        
        QueueAction(() =>
        {
            BattleController.Instance.battleCanvas.battleInfoPanel.HideInfo();
        });
    }

    public async Task<IEnumerable<EffectResult>> ReceiveSkill(IBattler source, Skill skill)
    {
        Debug.Log($"Recebendo Skill em {Character.Base.CharacterName}");
        
        var results = new List<EffectResult>();
        foreach (var effect in skill.Effects)
        {
            results.Add(await ReceiveEffect(source, skill, effect));
        }
        return results;
    }

    public async Task<EffectResult> ReceiveEffect(IBattler source, Skill skillSource, Effect effect)
    {
        EffectResult effectResult = null;

        await QueueActionAndAwait(() =>
        {
            effectResult = effect.ExecuteEffect(new SkillInfo
            {
                Skill = skillSource,
                Target = this,
                Source = source
            });
        });

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
                
                tasks.Add(BattleController.Instance.battleCanvas.ShowDamage(this, damageEffectResult.DamageDealt.ToString(), Color.white));
                
                await Task.WhenAll(tasks);
                break;
            }
            case HealEffect.HealEffectResult healEffectResult:
                await BattleController.Instance.battleCanvas.ShowDamage(this, healEffectResult.AmountHealed.ToString(),
                    Color.green);
                break;
        }

        return effectResult;
    }

    public async Task AfterSkill(IEnumerable<EffectResult> result)
    {
        //Fazer efeitos do que precisar aqui, quando precisar
    }
    #endregion
    
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
    
    public RectTransform RectTransform => transform as RectTransform;
}

#region PassiveInterfaces

public interface ITurnStartPassiveEffect
{
    Task OnTurnStart(IBattler battler);
}

#endregion

