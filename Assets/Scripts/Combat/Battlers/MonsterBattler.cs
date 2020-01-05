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
    [ReadOnly] public Encounter Encounter;
    [OnValueChanged("LoadBase")] public Monster MonsterBase;
    [SerializeField, ReadOnly] private GameObject monsterBattler;
    [SerializeField, ReadOnly] private Image image;
    private BattleController BattleController;


    #region Control

    private void Awake()
    {
        LoadBase();
    }

    private void Start()
    {
        BattleController = BattleController.Instance;
    }

    public void LoadBase()
    {
        if (MonsterBase == null)
            return;
        
        stats = MonsterBase.Stats;
        Skills = MonsterBase.Skills;
        MonsterAi = MonsterBase.MonsterAi;

        if (monsterBattler == null)
            monsterBattler = Instantiate(MonsterBase.MonsterBattler, RectTransform);
        if (image == null)
            image = monsterBattler.GetComponent<Image>();

        CurrentHp = Stats.MaxHp;
        CurrentEp = Stats.InitialEp;
    }

    private bool NoMonster => MonsterBase == null;

    #endregion
    
    #region Stats

    public string Name => MonsterBase.MonsterName;
    [FoldoutGroup("Stats"), ShowInInspector, PropertyOrder(999)] private Stats stats;
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
    
    public List<MonsterSkill> Skills;
    public MonsterAI MonsterAi;

    public bool Fainted => CurrentHp == 0;

    //[FoldoutGroup("Passives"), ShowInInspector, Sirenix.OdinInspector.ReadOnly] public List<BattlePassive> Passives { get; set; } = new List<BattlePassive>();

    #endregion
    
    #region TurnEvents

    public async Task TurnStart()
    {
        currentEp += Stats.EpGain;
        Debug.Log($"Começou o turno de {Name}");
    }

    public async Task TurnEnd()
    {
        Debug.Log($"Acabou o turno de {Name}");
    }
    
    public async Task<Turn> GetTurn()
    {
        if (Fainted)
            return new Turn();
        
        Debug.Log($"Começou a pegar o turno de {Name}");

        Turn turn = new Turn();

        await GameController.Instance.QueueActionAndAwait(() =>
        {
            var skill = MonsterAi.ChooseSkill(this);
            turn.Skill = skill;
            
            Debug.Log($"AI Escolheu {skill.SkillName}");
            
            //Debug, pega só um inimigo aleatório
            var possibleTargets = BattleController.Party.Where(partyMember => !Fainted).ToList();
            var possibleTarget = UnityEngine.Random.Range(0, possibleTargets.Count);
            turn.Targets = new [] {possibleTargets[possibleTarget]};
        });
        
        return turn;
    }

    public async Task ExecuteTurn(IBattler source, Skill skill, IEnumerable<IBattler> targets)
    {
        if (Skills != null)
        {
            GameController.Instance.QueueAction(() =>
            {
                BattleController.Instance.battleCanvas.BindActionArrow(RectTransform);
                foreach (var target in targets)
                {
                    BattleController.Instance.battleCanvas.BindTargetArrow(target.RectTransform);
                }
            });
            
            await BattleController.Instance.battleCanvas.battleInfoPanel.DisplayInfo(skill.SkillName);
            
            GameController.Instance.QueueAction(() =>
            {
                BattleController.Instance.battleCanvas.UnbindActionArrow();
                BattleController.Instance.battleCanvas.CleanTargetArrows();
            });
            
        }
    }

    //result = out
    public async Task<IEnumerable<EffectResult>> ReceiveSkill(IBattler source, Skill skill)
    {
        Debug.Log($"Recebendo skill em {Name}");
        var result = new List<EffectResult>();
        foreach (var effect in skill.Effects)
        {
            result.Add(await ReceiveEffect(source, skill, effect));
        }
        return result;
    }

    public async Task<EffectResult> ReceiveEffect(IBattler source, Skill skillSource, Effect effect)
    {
        EffectResult effectResult = null;
        
        await GameController.Instance.QueueActionAndAwait(() =>
        {
            effectResult = effect.ExecuteEffect(BattleController, skillSource, source, this);
        });

        //Ver pra mostrar Miss! quando o golpe errar, mostrar vermelho quando crita

        switch (effectResult)
        {
            case DamageEffect.DamageEffectResult damageEffectResult when !Fainted:
            {
                Task flash = DamageFlash();
                Task damage = BattleController.Instance.battleCanvas.ShowDamage(this, damageEffectResult.DamageDealt.ToString(), Color.white);

                await Task.WhenAll(flash, damage);
                break;
            }
            case DamageEffect.DamageEffectResult damageEffectResult:
            {
                Task fade = Fade();
                Task damage = BattleController.Instance.battleCanvas.ShowDamage(this, damageEffectResult.DamageDealt.ToString(), Color.white);

                await Task.WhenAll(fade, damage);
                break;
            }
            case HealEffect.HealEffectResult healEffectResult:
            {
                await BattleController.Instance.battleCanvas.ShowDamage(this, healEffectResult.AmountHealed.ToString(),
                    Color.green);
                break;
            } 
        }

        return effectResult;
    }

    public async Task AfterSkill(IEnumerable<EffectResult> result)
    {
        //Processar o que aconteceu quando usou a skill
    }

    #endregion
    
    private async Task DamageFlash()
    {
        await GameController.Instance.PlayCoroutine(DamageBlinkCoroutine());
    }

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

    private async Task Fade()
    {
        await GameController.Instance.PlayCoroutine(FadeCoroutine());
    }
    
    private IEnumerator FadeCoroutine(float speed = 0.05f)
    {
        while (image.color.a > 0)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, image.color.a - speed);
            yield return new WaitForFixedUpdate();
        }
    }
    
    public RectTransform RectTransform => transform as RectTransform;
}