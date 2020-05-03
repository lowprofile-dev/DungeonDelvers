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
    [ReadOnly] public GameObject monsterBattler;
    [ReadOnly] public Image image;
    private BattleController BattleController;
    
    #region Control
    protected override void Awake()
    {
        base.Awake();
        //LoadBase();
    }

    private void Start()
    {
        BattleController = BattleController.Instance;
    }

    public void LoadEncounterMonster(EncounterMonster monster)
    {
        MonsterBase = monster.Monster;

        var minLevel = monster.MinLevel;
        var maxLevel = monster.MaxLevel;

        var level = GameController.Instance.Random.Next(minLevel, maxLevel);

        Level = level;

        BaseStats = MonsterBase.Stats;
        BonusStats = MonsterBase.StatLevelVariance * (Level - MonsterBase.BaseLevel);

        Skills = MonsterBase.Skills; //Ver unlocks
        MonsterAi = MonsterBase.MonsterAi;
        
        monsterBattler = Instantiate(MonsterBase.MonsterBattler, RectTransform);
        image = monsterBattler.GetComponent<Image>();
        // monsterBattler = gameObject;
        // image = GetComponent<Image>();

        CurrentHp = Stats.MaxHp;
        CurrentEp = Stats.InitialEp;
        
        BattleDictionary = new Dictionary<object, object>();
        Passives = MonsterBase.Passives;
        StatusEffectInstances = new List<StatusEffectInstance>();
        BattlerName = MonsterBase.MonsterName;
        HitSound = MonsterBase.HitSound;
        
        Debug.Log($"Inicializado Lv.{level} {BattlerName}");
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
                var hitSound = PlayHitSound();
                var hasCrit = effectResult.skillInfo.HasCrit;
                var time = hasCrit ? 1.2f : 0.9f;
                var color = hasCrit ? Color.red : Color.white;
                Task damage = BattleController.Instance.battleCanvas.ShowSkillResultAsync(this,
                    damageEffectResult.DamageDealt.ToString(), color, time);
                Task blink = PlayCoroutine(DamageBlinkCoroutine());
                await Task.WhenAll(damage, blink, hitSound);
                break;
            }
            case DamageEffect.DamageEffectResult damageEffectResult:
            {
                var hitSound = PlayHitSound();
                var hasCrit = effectResult.skillInfo.HasCrit;
                var time = hasCrit ? 1.2f : 0.9f;
                var color = hasCrit ? Color.red : Color.white;
                Task damage = BattleController.Instance.battleCanvas.ShowSkillResultAsync(this,
                    damageEffectResult.DamageDealt.ToString(), color, time);
                Task fade = Fade();
                await Task.WhenAll(fade, damage, hitSound);
                break;
            }
            case HealEffect.HealEffectResult healEffectResult:
            {
                await BattleController.Instance.battleCanvas.ShowSkillResultAsync(this, healEffectResult.AmountHealed.ToString(),
                    Color.green);
                break;
            } 
            case MultiHitDamageEffect.MultiHitDamageEffectResult multiHitDamageEffectResult:
            {
                var tasks = new List<Task>();
                tasks.Add(MultiHitTask(multiHitDamageEffectResult, 1f));
                if (Fainted) tasks.Add(Fade());
                await Task.WhenAll(tasks);
                break;
            }
        }
    }

    #endregion

    #region Animations

    protected virtual async Task ShowDamageAndFlash(int damageAmount, bool isCrit)
    {
        await PlayCoroutine(ShowDamageAndFlashCoroutine(damageAmount, isCrit));
    }

    protected virtual IEnumerator ShowDamageAndFlashCoroutine(int damageAmount, bool isCrit)
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
    
    protected virtual async Task ShowDamage(int damageAmount, bool isCrit)
    {
        await PlayCoroutine(ShowDamageCoroutine(damageAmount, isCrit), this);
    }

    protected virtual IEnumerator ShowDamageCoroutine(int damageAmount, bool isCrit)
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

    protected virtual async Task DamageFlash()
    {
        await PlayCoroutine(DamageBlinkCoroutine());
    }

    protected virtual IEnumerator DamageBlinkCoroutine()
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

    protected virtual async Task Fade()
    {
        await PlayCoroutine(FadeCoroutine());
    }
    
    protected virtual IEnumerator FadeCoroutine(float speed = 0.05f)
    {
        while (image.color.a > 0)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, image.color.a - speed);
            yield return new WaitForFixedUpdate();
        }
    }

    protected virtual async Task MultiHitTask(MultiHitDamageEffect.MultiHitDamageEffectResult multiHitDamageEffectResult, float duration)
    {
        var damage = multiHitDamageEffectResult.TotalDamageDealt;
        var hits = multiHitDamageEffectResult.HitResults;
        var hitCount = hits.Count;
        var damageString = "";
        float segment = duration / hitCount;
                    
        Ref<(string text, Color color)> multiHitInfo = new Ref<(string, Color)>((String.Empty, multiHitDamageEffectResult.skillInfo.HasCrit ? Color.red : Color.white));
        var task = BattleController.Instance.battleCanvas.ShowModifiableSkillResultAsync(this, multiHitInfo,
            duration + 0.2f);

        var leftoverTasks = new List<Task>();
        leftoverTasks.Add(task);
                    
        for (int i = 0; i < hitCount; i++)
        {
            damageString += $"{hits[i].DamageDealt}\n";
            multiHitInfo.Instance.text = damageString;
            if (HitSound != null) leftoverTasks.Add(QueueActionAndAwait(() => AudioSource.PlayOneShot(HitSound)));
            if (damage > 0 && !Fainted) leftoverTasks.Add(DamageFlash());
            await Task.Delay((int) (segment*1000));
        }
                    
        await Task.WhenAll(leftoverTasks);
    }
    #endregion
}