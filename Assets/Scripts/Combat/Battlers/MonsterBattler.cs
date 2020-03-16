using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using SkredUtils;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;
// ReSharper disable RedundantAssignment

public class MonsterBattler : Battler
{
    [ReadOnly] public Encounter Encounter;
    [OnValueChanged("LoadBase")] public Monster MonsterBase;
    [SerializeField, ReadOnly] private GameObject monsterBattler;
    [SerializeField, ReadOnly] private Image image;
    private BattleController BattleController;
    
    #region Control
    private void Awake()
    {
        //LoadBase();
    }

    private void Start()
    {
        BattleController = BattleController.Instance;
    }

    public void LoadBase()
    {
        if (MonsterBase == null)
            return;

        var level = UnityEngine.Random.Range(MonsterBase.BaseLevel - MonsterBase.LevelVariance,
            MonsterBase.BaseLevel + MonsterBase.LevelVariance + 1);

        Level = level;
        
        //Stats = MonsterBase.Stats + MonsterBase.StatLevelVariance*(Level-MonsterBase.BaseLevel);
        BaseStats = MonsterBase.Stats;
        BonusStats = MonsterBase.StatLevelVariance * (Level - MonsterBase.BaseLevel);

        Skills = MonsterBase.Skills;
        MonsterAi = MonsterBase.MonsterAi;

        if (monsterBattler == null)
            monsterBattler = Instantiate(MonsterBase.MonsterBattler, RectTransform);
        if (image == null)
            image = monsterBattler.GetComponent<Image>();

        CurrentHp = Stats.MaxHp;
        CurrentEp = Stats.InitialEp;
        
        BattleDictionary = new Dictionary<object, object>();
        Passives = MonsterBase.Passives;
        StatusEffectInstances = new List<StatusEffectInstance>();
        BattlerName = MonsterBase.MonsterName;
        
        Debug.Log($"Inicializado Lv.{level} {BattlerName}");
    }

    private bool NoMonster => MonsterBase == null;

    #endregion
    
    #region Stats

    [FoldoutGroup("Stats"), SerializeField] private int currentHp;
    public override int CurrentHp
    {
        get => currentHp;
        set
        {
            currentHp = value;
            currentHp = Mathf.Clamp(currentHp, 0, Stats.MaxHp);
        }
    }
    [FoldoutGroup("Stats"), SerializeField] private int currentEp;
    public override int CurrentEp
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

    #endregion
    
    #region TurnEvents

    public override async Task<Turn> GetTurn()
    {
        if (Fainted || MonsterAi == null)
            return null;
        
        Debug.Log($"Começou a pegar o turno de {BattlerName}");

        Turn turn = new Turn();

        await QueueActionAndAwait(() =>
        {
            turn = MonsterAi.BuildTurn(this);
        });
        
        return turn;
    }

    protected override async Task AnimateTurn(Turn turn)
    {
        var targets = turn.Targets;
        var skill = turn.Skill;

        if (Skills != null)
        {
            QueueAction(() =>
            {
                BattleController.Instance.battleCanvas.BindActionArrow(RectTransform);
                foreach (var target in targets)
                {
                    BattleController.Instance.battleCanvas.BindTargetArrow(target.RectTransform);
                }
            });
            
            await BattleController.Instance.battleCanvas.battleInfoPanel.DisplayInfo(skill.SkillName);
            
            QueueAction(() =>
            {
                BattleController.Instance.battleCanvas.UnbindActionArrow();
                BattleController.Instance.battleCanvas.CleanTargetArrows();
            });
            
        } 
    }

    protected override async Task AnimateEffectResult(EffectResult effectResult)
    {
        switch (effectResult)
        {
            case DamageEffect.DamageEffectResult damageEffectResult when !Fainted:
            {
                var time = effectResult.skillInfo.HasCrit ? 1.4f : 1f;
//                await ShowDamageAndFlash(damageEffectResult.DamageDealt, effectResult.skillInfo.HasCrit);
                Task damage = BattleController.Instance.battleCanvas.ShowSkillResult(this,
                    damageEffectResult.DamageDealt.ToString(), Color.white, time);
                Task blink = PlayCoroutine(DamageBlinkCoroutine());

                await Task.WhenAll(damage, blink);
                break;
            }
            case DamageEffect.DamageEffectResult damageEffectResult:
            {
                var time = effectResult.skillInfo.HasCrit ? 1.4f : 1f;
                //Task damage = ShowDamage(damageEffectResult.DamageDealt, effectResult.skillInfo.HasCrit);
                Task damage = BattleController.Instance.battleCanvas.ShowSkillResult(this,
                    damageEffectResult.DamageDealt.ToString(), Color.white, time);
                Task fade = Fade();
                
                await Task.WhenAll(fade, damage);
                break;
            }
            case HealEffect.HealEffectResult healEffectResult:
            {
                await BattleController.Instance.battleCanvas.ShowSkillResult(this, healEffectResult.AmountHealed.ToString(),
                    Color.green);
                break;
            } 
            case MultiHitDamageEffect.MultiHitDamageEffectResult multiHitDamageEffectResult:
            {
                //Botar um pequeno delay entre cada hit do dano graficamente depois
                var tasks = new List<Task>();

                var damage = multiHitDamageEffectResult.TotalDamageDealt;

                if (damage == 0)
                {
                    //tasks.Add(ShowDamage(damage,effectInfo.SkillInfo.HasCrit));
                } else if (!Fainted)
                {
                    //tasks.Add(ShowDamageAndFlash(damage, effectInfo.SkillInfo.HasCrit));
                    tasks.Add(DamageFlash());
                }
                else
                {
                    //tasks.Add(ShowDamage(damage,effectInfo.SkillInfo.HasCrit));
                    tasks.Add(Fade());
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

    private async Task ShowDamageAndFlash(int damageAmount, bool isCrit)
    {
        await PlayCoroutine(ShowDamageAndFlashCoroutine(damageAmount, isCrit));
    }

    private IEnumerator ShowDamageAndFlashCoroutine(int damageAmount, bool isCrit)
    {
        var damageInstance = Instantiate(BattleController.Instance.battleCanvas.DamagePrefab, BattleController.Instance.battleCanvas.transform);
        damageInstance.transform.position = transform.position + new Vector3(0, 100, 0);

        var damageComponent = damageInstance.GetComponent<DamageText>();
        if (isCrit)
            damageComponent.SetupDamageText(damageAmount.ToString(),Color.red, 4);
        else
            damageComponent.SetupDamageText(damageAmount.ToString(), Color.white);
        
        var normalColor = image.color;
        var blinkingColor = new Color(image.color.r, image.color.g, image.color.b, 0.3f);
        
        for (int i = 0; i < 5; i++)
        {
            image.color = blinkingColor;
            yield return new WaitForSeconds(0.05f);
            image.color = normalColor;
            yield return new WaitForSeconds(0.05f);
        }
        
        yield return new WaitForSeconds(1);
        
        Destroy(damageInstance);
    }
    
    private async Task ShowDamage(int damageAmount, bool isCrit)
    {
        await PlayCoroutine(ShowDamageCoroutine(damageAmount, isCrit), this);
    }

    private IEnumerator ShowDamageCoroutine(int damageAmount, bool isCrit)
    {
        var damageInstance = Instantiate(BattleController.Instance.battleCanvas.DamagePrefab, BattleController.Instance.battleCanvas.transform);
        damageInstance.transform.position = transform.position + new Vector3(0, 100, 0);

        var damageComponent = damageInstance.GetComponent<DamageText>();
        if (isCrit)
            damageComponent.SetupDamageText(damageAmount.ToString(),Color.red, 4);
        else
            damageComponent.SetupDamageText(damageAmount.ToString(), Color.white);
        
        yield return new WaitForSeconds(1.4f);
        
        Destroy(damageInstance);
    }

    // private async Task ShowHeal()
    // {
        
    // }
    
    private async Task DamageFlash()
    {
        await PlayCoroutine(DamageBlinkCoroutine());
    }

    private IEnumerator DamageBlinkCoroutine()
    {
        var normalColor = image.color;
        var blinkingColor = new Color(image.color.r, image.color.g, image.color.b, 0.3f);

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
        await PlayCoroutine(FadeCoroutine());
    }
    
    private IEnumerator FadeCoroutine(float speed = 0.05f)
    {
        while (image.color.a > 0)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, image.color.a - speed);
            yield return new WaitForFixedUpdate();
        }
    }

    #endregion
}