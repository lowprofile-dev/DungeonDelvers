using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

public abstract class Battler : AsyncMonoBehaviour
{
    public Dictionary<object, object> BattleDictionary { get; set; } = new Dictionary<object, object>();

    #region Fields

    public virtual string BattlerName { get; protected set; }
    public virtual int Level { get; protected set; }
    [FoldoutGroup("Stats")] public abstract int CurrentHp { get; set; }
    [FoldoutGroup("Stats")] public abstract int CurrentEp { get; set; }
    public bool Fainted => CurrentHp == 0;

    #endregion

    #region Stats

    private bool _statsDirty = true;
    [FoldoutGroup("Stats"), ShowInInspector] private Stats baseStats;
    [FoldoutGroup("Stats"), ShowInInspector] private Stats bonusStats;
    [FoldoutGroup("Stats"), ShowInInspector] private Stats totalStats;

    public Stats BaseStats
    {
        get => baseStats;
        set
        {
            _statsDirty = true;
            baseStats = value;
        }
    }
    public Stats BonusStats
    {
        get => bonusStats;
        set
        {
            _statsDirty = true;
            bonusStats = value;
        }
    }
    public Stats Stats
    {
        get
        {
            if (_statsDirty)
            {
                totalStats = baseStats + bonusStats;
            }
            return totalStats;
        }
    }
    
    [TabGroup("Passives"), ShowInInspector, ReadOnly] public virtual List<Passive> Passives { get; protected set; }
    [TabGroup("Status Effects")] public virtual List<StatusEffectInstance> StatusEffectInstances { get; set; }
    
    public virtual List<PassiveEffect> PassiveEffects =>
        Passives
            .SelectMany(passive => passive.Effects)
            .Concat(
                StatusEffectInstances
                    .SelectMany(statusEffectInstance => statusEffectInstance.StatusEffect.Effects))
            .OrderByDescending(passiveEffect => passiveEffect.Priority)
            .ToList();

    public virtual RectTransform RectTransform => transform as RectTransform;

    #endregion

    #region BattleEvents

    public async Task TurnStart()
    {
        CurrentEp += Stats.EpGain;
        Debug.Log($"Começou o turno de {BattlerName}");

        var expiredStatusEffects = StatusEffectInstances
            .Where(statusEffect => statusEffect.TurnDuration <= 0)
            .ToArray();

        await Task.WhenAll(expiredStatusEffects
            .Select(RemoveStatusEffect));

        var effectsFromPassives = Passives
            .SelectMany(passive => passive.Effects
                .Where(effect => effect is ITurnStartPassiveEffect)
                .Select(effect => (passive as object, effect))).ToArray();

        var effectsFromStatuses = StatusEffectInstances
            .SelectMany(instance => instance.StatusEffect.Effects
                .Where(effect => effect is ITurnStartPassiveEffect)
                .Select(effect => (instance as object, effect))).ToArray();

        var turnStartEffects = effectsFromPassives
            .Concat(effectsFromStatuses)
            .OrderByDescending(turnStartEffect => turnStartEffect.effect.Priority)
            .Select(turnStartEffect => (turnStartEffect.Item1,turnStartEffect.Item2 as ITurnStartPassiveEffect))
            .ToArray();

        foreach (var turnStartPassive in turnStartEffects)
        {
            if (turnStartPassive.Item1 is Passive passive)
            {
                await turnStartPassive.Item2.OnTurnStart(new PassiveEffectInfo
                {
                    PassiveEffectSourceName = passive.PassiveName,
                    Source = this,
                    Target = this
                });
            }
            else if (turnStartPassive.Item1 is StatusEffectInstance statusEffectInstance)
            {
                await turnStartPassive.Item2.OnTurnStart(new PassiveEffectInfo
                {
                    PassiveEffectSourceName = statusEffectInstance.StatusEffect.StatusEffectName,
                    Source = statusEffectInstance.Source,
                    Target = statusEffectInstance.Target
                });
            }
            else
            {
                throw new ArgumentException();
            }
        }
    }

    public async Task TurnEnd()
    {
        StatusEffectInstances.ForEach(instance => { instance.TurnDuration--; });
        Debug.Log($"Acabou o turno de {BattlerName}.");
    }

    public abstract Task<Turn> GetTurn();
    public async Task ExecuteTurn(Turn turn)
    {
        Debug.Log($"Executando o turno de {BattlerName}.");

        if (turn == null)
            return;

        if (turn.Skill.EpCost > CurrentEp)
        {
            Debug.LogError(
                $"{BattlerName} tentou usar uma skill com custo maior que o EP Atual. SkillName: {turn.Skill.SkillName}, SkillCost: {turn.Skill.EpCost}, CurrentEp: {CurrentEp}");
        }

        CurrentEp -= turn.Skill.EpCost;

        QueueAction(() => { BattleController.Instance.battleCanvas.battleInfoPanel.ShowInfo(turn.Skill.SkillName); });

        await AnimateTurn(turn);
        
        QueueAction(() => {BattleController.Instance.battleCanvas.battleInfoPanel.HideInfo();});
    }

    public async Task<IEnumerable<EffectResult>> ReceiveSkill(Battler source, Skill skill)
    {
        Debug.Log($"Recebendo skill em {BattlerName}");
        bool hasHit;
        
        if (skill.TrueHit)
        {
            Debug.Log($"{source.BattlerName} acertou {BattlerName} com {skill.SkillName} por ter True Hit");
            hasHit = true;
        }
        else
        {
            var accuracy = source.Stats.Accuracy + skill.AccuracyModifier;
            var evasion = Stats.Evasion;

            var hitChance = accuracy - evasion;

            var rng = GameController.Instance.Random.NextDouble();

            hasHit = rng <= hitChance;
            
            Debug.Log($"{source.BattlerName} {(hasHit ? "acertou":"errou")} {BattlerName} com {skill.SkillName} -- Acc: {accuracy}, Eva: {evasion}, Rng: {rng:F3}");
        }
        if (hasHit)
        {
            var results = new List<EffectResult>();

            bool hasCrit;

            if (!skill.CanCritical)
            {
                hasCrit = false;
                Debug.Log($"{source.BattlerName} não critou {BattlerName} com {skill.SkillName} por não poder critar.");
            }
            else
            {
                var critAccuracy = source.Stats.CritChance + skill.CriticalModifier;
                var critEvasion = Stats.CritAvoid;

                var critChance = critAccuracy - critEvasion;

                var rng = GameController.Instance.Random.NextDouble();

                hasCrit = rng <= critChance;
                
                Debug.Log($"{source.BattlerName} {(hasCrit ? "":"não")} critou {BattlerName} com {skill.SkillName} -- Acc: {critAccuracy}, Eva: {critEvasion}, Rng: {rng:F3}");
            }
            
            var skillInfo = new SkillInfo
            {
                HasCrit = hasCrit,
                Skill = skill,
                Source = source,
                Target = this
            };

            var effects = hasCrit ? skill.CriticalEffects : skill.Effects;
            
            foreach (var effect in effects)
            {
                results.Add(
                    await ReceiveEffect(new EffectInfo
                    {
                        SkillInfo = skillInfo,
                        Effect = effect
                    }));
            }
            return results;
        }
        else
        {
            await BattleController.Instance.battleCanvas.ShowSkillResult(this, "Miss!", Color.white);
            return new EffectResult[] { };
            //retonar missresult depois(?)
        }
    }

    public async Task<EffectResult> ReceiveEffect(EffectInfo effectInfo)
    {
        EffectResult effectResult = null;

        await QueueActionAndAwait(() =>
        {
            effectResult = effectInfo.Effect.ExecuteEffect(effectInfo.SkillInfo);
        });

        await AnimateEffectResult(effectResult);

        return effectResult;
    }

    public async Task AfterSkill(IEnumerable<EffectResult> results)
    {
    }

    protected abstract Task AnimateTurn(Turn turn);
    protected abstract Task AnimateEffectResult(EffectResult effectResult);

    #endregion

    #region Functions

    public void RecalculateStats()
    {
        
    }
    
    public async Task ApplyStatusEffect(StatusEffectInstance statusEffectInstance)
    {
        StatusEffectInstances.Add(statusEffectInstance);
        var effects = statusEffectInstance.StatusEffect.Effects
            .Where(effect => effect is IOnApplyPassiveEffect)
            .Cast<IOnApplyPassiveEffect>();

        foreach (var effect in effects)
        {
            await QueueActionAndAwait(()=>effect.OnApply(this));
        }
    }

    public async Task RemoveStatusEffect(StatusEffectInstance statusEffectInstance)
    {
        var removed = StatusEffectInstances.Remove(statusEffectInstance);
        
        if (!removed)
            return;

        var effects = statusEffectInstance.StatusEffect.Effects
            .Where(effect => effect is IOnApplyPassiveEffect)
            .Cast<IOnApplyPassiveEffect>();

        foreach (var effect in effects)
        {
            await QueueActionAndAwait(()=>effect.OnUnapply(this));
        }
    }

    #endregion
}

#region PassiveInterfaces

public interface ITurnStartPassiveEffect
{
    Task OnTurnStart(PassiveEffectInfo passiveEffectInfo);
}

public interface IStatModifierPassiveEffect
{
    void AddBonuses(Battler battler, ref Stats bonuses);
}

public interface IOnApplyPassiveEffect
{
    void OnApply(Battler battler);
    void OnUnapply(Battler battler);
}

#endregion