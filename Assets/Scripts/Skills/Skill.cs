using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Skill/Skill")]
public class Skill : SerializableAsset
{
    public string SkillName = "";
    [AssetIcon] public Sprite SkillIcon;
    [FormerlySerializedAs("EpCost")] public int ApCost;
    public Element Element;
    [TextArea] public string SkillDescription;
    public TargetType Target;
    public List<Effect> Effects = new List<Effect>();
    [ShowIf("CanCritical")] public List<Effect> CriticalEffects = new List<Effect>();
    
    [TabGroup("Effect Animations")]public DD.Skill.Animation.Animation SkillAnimation = null;
    
    public bool TrueHit = false;
    public bool CanBeSilenced;
    [OnValueChanged("_initializeCriticalEffects")] public bool CanCritical;

    [Range(-1,1), HideIf("TrueHit")] public float AccuracyModifier = 0f;
    [Range(-1, 1), ShowIf("CanCritical")] public float CriticalModifier = 0f;
    
    //Ver se necessario mais depois, eg. Raise-equivalent -> one dead ally
    public enum TargetType
    {
        Any,
        OneEnemy,
        OneAlly,
        AllEnemies,
        AllAllies,
        All,
        Self
    }
    
#if UNITY_EDITOR
    private void _initializeCriticalEffects()
    {
        if (CanCritical && (CriticalEffects == null || !CriticalEffects.Any()))
        {
            CriticalEffects = new List<Effect>(Effects.Select(e => (Effect)e.Clone()));
        }
    }
#endif
}

public struct SkillInfo
{
    public Battler Target;
    public Battler Source;
    [CanBeNull] public Skill Skill;
    public bool HasCrit;
}

public struct EffectInfo
{
    public SkillInfo SkillInfo;
    public Effect Effect;
}