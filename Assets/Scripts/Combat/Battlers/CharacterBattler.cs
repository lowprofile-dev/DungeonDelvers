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
// ReSharper disable RedundantAssignment

public class CharacterBattler : SerializedMonoBehaviour, IBattler
{
    public Animator animator;
    public Character Character;
    
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

            if (currentHp == 0)
            {
                UpdateAnimator();
            }
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
            animator.Play(animation.ToString());
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

    public void Play(CharacterBattlerAnimation animation)
    {
        animator.Play(animation.ToString());
    }

    public void UpdateAnimator(bool? weaponOverride = null, bool? faintedOverride = null)
    {
        var weapon = weaponOverride.HasValue ? weaponOverride.Value : Character.Weapon != null;
        var fainted = faintedOverride.HasValue ? faintedOverride.Value : Fainted;
        
        animator.SetBool("HasWeapon", weapon);
        animator.SetBool("Fainted", fainted);
    }
    
    public enum CharacterBattlerAnimation
    {
        Idle,
        IdleNoWeapon,
        Attack,
        Damage,
        Fainted
    } 

    #endregion
    
    #region TurnEvents
    public async Task TurnStart(BattleController battle)
    {
        //Fazer efeitos do que precisar aqui, quando precisar
        currentEp += Stats.EpGain;
        Debug.Log($"Começou turno de {Character.Base.CharacterName}");
    }

    public async Task TurnEnd(BattleController battle)
    {
        //Fazer efeitos do que precisar aqui, quando precisar
        Debug.Log($"Acabou o turno de {Character.Base.CharacterName}");
    }

    public async Task<Turn> GetTurn(BattleController battle)
    {
        Debug.Log($"Fazendo o turno de {Character.Base.CharacterName}");

        if (Fainted)
            return new Turn()
            {
                Skill = null
            };
        
        var turn = await battle.battleCanvas.GetTurn(this);

        return turn;
    }

    public async Task ExecuteTurn(BattleController battle, IBattler source, Skill skill, IEnumerable<IBattler> targets)
    {
        Debug.Log($"Executando o turno de {Character.Base.CharacterName}");
        
        foreach (var effect in skill.Effects)
        {
            if (effect is DamageEffect)
            {
                await AsyncPlayAndWait(CharacterBattlerAnimation.Attack);
            }
        }
    }

    public async Task<IEnumerable<EffectResult>> ReceiveSkill(BattleController battle, IBattler source, Skill skill)
    {
        Debug.Log($"Recebendo Skill em {Character.Base.CharacterName}");
        
        var results = new List<EffectResult>();
        foreach (var effect in skill.Effects)
        {
            EffectResult effectResult = null;

            await GameController.Instance.QueueActionAndAwait(() =>
            {
                effectResult = effect.ExecuteEffect(battle, skill, source, this);
            });

            if (effectResult is DamageEffect.DamageEffectResult damageEffectResult)
            {
                await AsyncPlayAndWait(CharacterBattlerAnimation.Damage);
                
                if (damageEffectResult.DamageDealt > 0)
                    await BattleController.Instance.battleCanvas.ShowDamage(this, damageEffectResult.DamageDealt);
            }
            results.Add(effectResult);
        }
        
        return results;
    }
    
    public async Task AfterSkill(BattleController battleController, IEnumerable<EffectResult> result)
    {
        //Fazer efeitos do que precisar aqui, quando precisar
    }
    #endregion
    
    public RectTransform RectTransform => transform as RectTransform;
    
}