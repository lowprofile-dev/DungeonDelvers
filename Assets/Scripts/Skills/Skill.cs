using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Skill/Skill")]
public class Skill : SerializableAsset
{
    public string SkillName = "";
    [AssetIcon] public Sprite SkillIcon;
    public int EpCost;
    [TextArea] public string SkillDescription;
    public TargetType Target;
    public List<Effect> Effects = new List<Effect>();
    [ShowIf("CanCritical")] public List<Effect> CriticalEffects = new List<Effect>();
    
    public SkillAnimation SkillAnimation = null;
    
    public bool TrueHit = false;
    public bool CanBeSilenced;
    public bool CanCritical;
    
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
}

public struct SkillInfo
{
    public Battler Target;
    public Battler Source;
    public Skill Skill;
    public bool HasCrit;
}

public struct EffectInfo
{
    public SkillInfo SkillInfo;
    public Effect Effect;
}