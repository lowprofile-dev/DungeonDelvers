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
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = System.Random;
// ReSharper disable RedundantAssignment

// public abstract class MonsterBattlerBase<T> : Battler where T : Monster
// {
//     public abstract void LoadMonsterBase(T monsterBase, int level);
// }

public class MonsterBattler : Battler
{
    [ReadOnly] public Encounter Encounter;
    [OnValueChanged("LoadBase")] public Monster MonsterBase;
    [ReadOnly] public GameObject monsterBattler;
    [ReadOnly] public Image image;
    private BattleController BattleController;

    private ISkillSelector SkillAi;
    private ITargetSelector TargeterAi;
    public override IEnumerable<Skill> SkillList => Skills;

    #region Control
    protected virtual void Start()
    {
        BattleController = BattleController.Instance;
    }

    public virtual void LoadMonsterBase(Monster monsterBase, int level)
    {
        MonsterBase = monsterBase;
        Level = level;

        // BaseStats = MonsterBase.Stats;
        // BonusStats = MonsterBase.StatLevelVariance * (Level - MonsterBase.BaseLevel);

        // Skills = MonsterBase.Skills;
        // SkillAi = MonsterBase.SkillAi;
        
        StatusEffectInstances = new List<StatusEffectInstance>();
        RecalculateStats();
        
        TargeterAi = MonsterBase.TargeterAi;

        monsterBattler = Instantiate(MonsterBase.MonsterBattler, RectTransform);
        image = monsterBattler.gameObject.Ensure<Image>();

        CurrentHp = Stats.MaxHp;
        CurrentAp = GameSettings.Instance.InitialAp;
        
        BattleDictionary = new Dictionary<object, object>();
        // Passives = MonsterBase.Passives;
        // StatusEffectInstances = new List<StatusEffectInstance>();
        BattlerName = MonsterBase.MonsterName;
        HitSound = MonsterBase.HitSound;
        
        Debug.Log($"Inicializado Lv.{level} {BattlerName}");
    }

    [Obsolete]
    public virtual void LoadEncounterMonster(EncounterMonster monster)
    {
        throw new NotImplementedException();
    }
    
    [Obsolete]
    public virtual void LoadBase()
    {
        throw new NotImplementedException();
    }

    protected bool NoMonster => MonsterBase == null;

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
    [FormerlySerializedAs("currentEp")] [FoldoutGroup("Stats"), SerializeField] private int currentAp;
    public override int CurrentAp
    {
        get => currentAp;
        set
        {
            currentAp = value;
            Mathf.Clamp(currentAp, 0, 100);
        }
    }
    
    public List<MonsterSkill> Skills;

    public override void RecalculateStats()
    {
        Passives = MonsterBase.Passives;
        Skills = MonsterBase.Skills;
        var baseStats = MonsterBase.Stats;
        
        
        var bonusStats = MonsterBase.StatLevelVariance * (Level - MonsterBase.BaseLevel);
        foreach (IMonsterCalculateBonusStatsListener listener in PassiveEffects.Where(pE =>
            pE is IMonsterCalculateBonusStatsListener))
        {
            listener.Apply(this, ref bonusStats);
        }

        Stats = baseStats + bonusStats;
        Debug.Log($"{BattlerName} stats recalculated");
    }

    #endregion
    
    #region TurnEvents

    public override async Task<Turn> GetTurn()
    {
        if (Fainted || SkillAi == null || TargeterAi == null)
            return null;
        
        Debug.Log($"Começou a pegar o turno de {BattlerName}");

        Turn turn = new Turn();

        await QueueActionAndAwait(() =>
        {
            //turn = MonsterAi.BuildTurn(this);
            var skill = SkillAi.GetSkill(this);
            if (skill == null)
            {
                turn = null;
                return;
            }
            var targets = TargeterAi.GetTargets(this,skill);
            if (targets == null)
            {
                turn = null;
                return;
            }
            turn = new Turn
            {
                Skill = skill,
                Targets = targets
            };
        });
        
        return turn;
    }

    protected override async Task AnimateTurn(Turn turn)
    {
        var targets = turn.Targets;
        var skill = turn.Skill;

        if (skill != null)
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
            
            if (skill.SkillAnimation != null)
            {
                await skill.SkillAnimation.PlaySkillAnimation(this, turn.Targets);
            }
        } 
    }

    protected override async Task AnimateEffectResult(EffectResult effectResult)
    {
        switch (effectResult)
        {
            case BlockPassiveEffect.BlockedResult _:
            {
                await BattleController.Instance.battleCanvas.ShowSkillResultAsync(this, "Blocked!", Color.white, 0.8f);
                break;
            }
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
        var color = image.color;
        var normalColor = new Color(color.r,color.g,color.b,1f);
        var blinkingColor = new Color(color.r, color.g, color.b, 0.3f);

        for (int i = 0; i < 5; i++)
        {
            image.color = blinkingColor;
            yield return new WaitForSeconds(0.05f);
            image.color = normalColor;
            yield return new WaitForSeconds(0.05f);
        }

        image.color = normalColor;
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

public interface IMonsterCalculateBonusStatsListener
{
    void Apply(MonsterBattler monster, ref Stats stats);
}