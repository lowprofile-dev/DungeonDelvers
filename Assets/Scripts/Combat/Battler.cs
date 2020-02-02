using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sirenix.Utilities;
using UnityEngine;

public abstract class Battler : AsyncMonoBehaviour
{
    public Dictionary<object, object> BattleDictionary = new Dictionary<object, object>();

    #region Fields

    public virtual string BattlerName { get; set; }
    public virtual int Level { get; set; }
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
            //await turnStartPassive.OnTurnStart(this);
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
        var result = new List<EffectResult>();

        foreach (var effect in skill.Effects)
        {
            result.Add(await ReceiveEffect(source, skill, effect));
        }

        return result;
    }

    public async Task<EffectResult> ReceiveEffect(Battler source, Skill skill, Effect effect)
    {
        EffectResult effectResult = null;

        await QueueActionAndAwait(() =>
        {
            // effectResult = effect.ExecuteEffect(new ActionInfo<Skill>
            // {
            //     ActionSource = skill,
            //     Source = source,
            //     Target = this
            // });
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

    // Task OnTurnStart(Battler battler);
}

#endregion