using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using SkredUtils;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// ReSharper disable RedundantAssignment

public class CharacterBattler : SerializedMonoBehaviour, IBattler
{
    public Animator animator;
    public Character Character;
    private BattleController BattleController; 
    private Image image;

    private void Start()
    {
        image = GetComponent<Image>();
        BattleController = BattleController.Instance;
    }

    public void Create(Character character)
    {
        Character = character;
        Stats = character.Stats;
        CurrentHp = character.CurrentHp;
        CurrentEp = Stats.InitialEp;
        Skills = character.Skills;
    }
    
    public void CommitChanges()
    {
        Character.CurrentHp = currentHp;
    }

    #region Stats

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
    [FoldoutGroup("Skills")] public List<Skill> Skills { get; private set; }
    //[FoldoutGroup("Passives"), ShowInInspector, Sirenix.OdinInspector.ReadOnly] public List<BattlePassive> Passives { get; set; } = new List<BattlePassive>();
    
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
        await GameController.Instance.QueueActionAndAwait(() =>
        {
            Play(animation);
        });
        
        await Task.Delay(5);

        bool? condition = null;

        Action evaluateCondition = () =>
        {
            condition = animator.GetCurrentAnimatorStateInfo(0).IsName(animation.ToString());
        };

        await GameController.Instance.QueueActionAndAwait(evaluateCondition);
        
        while (condition.HasValue && condition.Value == true)
        {
            await GameController.Instance.QueueActionAndAwait(evaluateCondition);
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
        //Fazer efeitos do que precisar aqui, quando precisar
        currentEp += Stats.EpGain;
        Debug.Log($"Começou turno de {Character.Base.CharacterName}");
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
        
        var turn = await BattleController.battleCanvas.GetTurn(this);

        return turn;
    }

    public async Task ExecuteTurn(IBattler source, Skill skill, IEnumerable<IBattler> targets)
    {
        Debug.Log($"Executando o turno de {Character.Base.CharacterName}");
        
        // foreach (var effect in skill.Effects)
        // {
        //     if (effect is DamageEffect)
        //     {
        //         await AsyncPlayAndWait(CharacterBattlerAnimation.Attack);
        //     }
        // }

        await AsyncPlayAndWait(skill.AnimationType);

        if (skill.SkillAnimation != null)
        {
            await skill.SkillAnimation.PlaySkillAnimation(source, targets);
        }
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

        await GameController.Instance.QueueActionAndAwait(() =>
        {
            effectResult = effect.ExecuteEffect(BattleController, skillSource, source, this);
        });

        switch (effectResult)
        {
            case DamageEffect.DamageEffectResult damageEffectResult:
            {
                if (damageEffectResult.DamageDealt > 0)
                {
                    Task damageAnimation =  AsyncPlayAndWait(CharacterBattlerAnimation.Damage);
                    Task damageBlink = GameController.Instance.PlayCoroutine(DamageBlinkCoroutine());

                    await Task.WhenAll(damageAnimation, damageBlink);
                }
                
                await BattleController.Instance.battleCanvas.ShowDamage(this, damageEffectResult.DamageDealt.ToString(), Color.white);
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