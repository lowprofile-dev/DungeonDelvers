using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using SkredUtils;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;
// ReSharper disable RedundantAssignment

public class MonsterBattler : SerializedMonoBehaviour, IBattler
{
    public string Name;
    private Image image;

    private void Awake()
    {
        CurrentHp = Stats.MaxHp;
        CurrentEp = Stats.InitialEp;
    }

    private void Start()
    {
        image = GetComponent<Image>();
    }

    #region Stats

    [FoldoutGroup("Stats"), SerializeField, PropertyOrder(999)] private Stats stats;
    public Stats Stats => stats;
    
    [FoldoutGroup("Stats"), SerializeField] private int currentHp;
    public int CurrentHp
    {
        get => currentHp;
        set
        {
            currentHp = value;
            currentHp = Mathf.Clamp(currentHp, 0, Stats.MaxHp);
        }
    }
    [FoldoutGroup("Stats"), SerializeField] private int currentEp;
    public int CurrentEp
    {
        get => currentEp;
        set
        {
            currentEp = value;
            Mathf.Clamp(currentEp, 0, 100);
        }
    }
    
    public Skill Skill;

    //[FoldoutGroup("Passives"), ShowInInspector, Sirenix.OdinInspector.ReadOnly] public List<BattlePassive> Passives { get; set; } = new List<BattlePassive>();

    #endregion
    
    #region TurnEvents

    public async Task TurnStart(BattleController battle)
    {
        currentEp += Stats.EpGain;
        Debug.Log($"Começou o turno de {Name}");
    }

    public async Task TurnEnd(BattleController battle)
    {
        Debug.Log($"Acabou o turno de {Name}");
    }
    
    public async Task<Turn> GetTurn(BattleController battle)
    {
        Debug.Log($"Começou a pegar o turno de {Name}");

        IBattler target = null;

        await GameController.Instance.QueueActionAndAwait(() =>
        {
            var possibleTargets = battle.Party;
            var possibleTarget = UnityEngine.Random.Range(0, possibleTargets.Count);
            target = possibleTargets[possibleTarget];
        });
        
        return new Turn()
        {
            Skill = Skill,
            Targets = new IBattler[]{target}
        };
    }

    public async Task ExecuteTurn(BattleController battle, IBattler source, Skill skill, IEnumerable<IBattler> targets)
    {
        if (Skill != null)
        {
            GameController.Instance.QueueAction(() =>
            {
                BattleController.Instance.battleCanvas.BindActionArrow(RectTransform);
                foreach (var target in targets)
                {
                    BattleController.Instance.battleCanvas.BindTargetArrow(target.RectTransform);
                }
            });
            
            await BattleController.Instance.battleCanvas.SkillUsePanel.ShowSkillInfoDuration(Skill);
            
            GameController.Instance.QueueAction(() =>
            {
                BattleController.Instance.battleCanvas.UnbindActionArrow();
                BattleController.Instance.battleCanvas.CleanTargetArrows();
            });
            
        }
    }

    //result = out
    public async Task<IEnumerable<EffectResult>> ReceiveSkill(BattleController battle, IBattler source, Skill skill)
    {
        Debug.Log($"Recebendo skill em {Name}");
        var result = new List<EffectResult>();
        foreach (var effect in skill.Effects)
        {
            EffectResult effectResult = null;

            await GameController.Instance.QueueActionAndAwait(() =>
            {
                effectResult = effect.ExecuteEffect(battle, skill, source, this);
            });
            
            result.Add(effectResult);

            if (effectResult is DamageEffect.DamageEffectResult damageEffectResult)
            {
                //await DamageFlash();
                await BattleController.Instance.battleCanvas.ShowDamage(this, damageEffectResult.DamageDealt);
            }
        }
        return result;
    }

    public async Task AfterSkill(BattleController battleController, IEnumerable<EffectResult> result)
    {
        //Processar o que aconteceu quando usou a skill
    }

    #endregion
    
    private async Task DamageFlash()
    {
        /*
        image.color = Color.red;
        await Task.Delay(100);
        image.color = Color.white;
        await Task.Delay(100);
        image.color = Color.red;
        await Task.Delay(100);
        image.color = Color.white;
        */
    }
    
    public RectTransform RectTransform => transform as RectTransform;
}