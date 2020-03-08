using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using UnityEditor.VersionControl;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/StatusEffect")]
public class StatusEffect : SerializableAsset
{
    public string StatusEffectName = "";
    [AssetIcon] public Sprite StatusEffectIcon;
    public StatusEffectType Type = StatusEffectType.None;
    public bool Hidden = false;
    public List<PassiveEffect> Effects = new List<PassiveEffect>();

    public void Apply(SkillInfo skillInfo,int turnDuration)
    {
        var instance = new StatusEffectInstance
        {
            Source = skillInfo.Source,
            Target = skillInfo.Target,
            StatusEffect = this,
            TurnDuration = turnDuration
        };
        
        //skillInfo.Target.StatusEffectInstances.Add(instance);
        skillInfo.Target.ApplyStatusEffect(instance);
    }
    
    public enum StatusEffectType
    {
        None,
        Enchantment
        //Poison
        //Sleep
        //etc
    }
}