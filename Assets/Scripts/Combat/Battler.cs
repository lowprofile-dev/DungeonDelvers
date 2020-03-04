using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sirenix.Utilities;
using UnityEngine;

public abstract class Battler : AsyncMonoBehaviour, IBattler
{
    #region tbi
    
    public string Name => "";
    
    public Task ExecuteTurn(IBattler source, Skill skill, IEnumerable<IBattler> targets)
    {
        throw new System.NotImplementedException();
    }

    public Task<IEnumerable<EffectResult>> ReceiveSkill(IBattler source, Skill skill)
    {
        throw new System.NotImplementedException();
    }

    
    #endregion
    
    
    public Dictionary<object, object> BattleDictionary { get; set; } = new Dictionary<object, object>();

    #region Fields

    public virtual string BattlerName { get; }
    public virtual int Level { get; }
    public virtual int CurrentHp { get; set; }
    public virtual int CurrentEp { get; set; }
    public bool Fainted => CurrentHp == 0;

    #endregion

    #region Stats

    public virtual Stats Stats { get; }
    public virtual List<Passive> Passives { get; }
    public virtual List<StatusEffect> StatusEffects { get; }
    public virtual RectTransform RectTransform { get; }

    #endregion

    #region BattleEvents

    public async Task TurnStart()
    {
        CurrentEp += Stats.EpGain;
        Debug.Log($"Começou o turno de {BattlerName}");

        var expiredStatusEffects = StatusEffects
            .Where(statusEffect => statusEffect.TurnDuration <= BattleController.Instance.CurrentTurn)
            .ToArray();

        expiredStatusEffects
            .ForEach(expired => StatusEffects.Remove(expired));

        var effectsFromPassives = Passives
            .SelectMany(passive => passive.Effects
                .Where(effect => effect is ITurnStartPassiveEffect));

        var effectsFromStatuses = StatusEffects
            .SelectMany(statusEffect => statusEffect.Effects
                .Where(effect => effect is ITurnStartPassiveEffect));

        var turnStartEffects = effectsFromPassives
            .Concat(effectsFromStatuses)
            .OrderByDescending(effect => effect.Priority)
            .Cast<ITurnStartPassiveEffect>()
            .ToArray();

        foreach (var turnStartPassive in turnStartEffects)
        {
            await turnStartPassive.OnTurnStart(this);
        }
    }

    public async Task TurnEnd()
    {
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
            
            Debug.Log($"{source.BattleDictionary} {(hasHit ? "acertou":"errou")} {BattlerName} com {skill.SkillName} -- Acc: {accuracy}, Eva: {evasion}, Rng: {rng}");
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
                
                Debug.Log($"{source.BattlerName} {(hasCrit ? "":"não")} critou {BattlerName} com {skill.SkillName} -- Acc: {critAccuracy}, Eva: {critEvasion}, Rng: {rng}");
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
}

#region PassiveInterfaces

public interface ITurnStartPassiveEffect
{
    Task OnTurnStart(IBattler battler);
    //Task OnTurnStart(Battler battler);
}

#endregion