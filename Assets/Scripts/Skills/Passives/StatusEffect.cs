using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using UnityEngine;

public class StatusEffect
{
    public string StatusEffectName = "";
    public Sprite StatusEffectIcon;
    public string GetName => StatusEffectName;
    public StatusEffectType Type = StatusEffectType.None;
    public bool Hidden = false;
    public List<PassiveEffect> Effects = new List<PassiveEffect>();
    public List<PassiveEffect> GetEffects => Effects;

    public void Apply(SkillInfo skillInfo,int turnDuration)
    {
        var instance = new StatusEffectInstance
        {
            Source = skillInfo.Source,
            Target = skillInfo.Target,
            StatusEffect = this,
            TurnDuration = turnDuration
        };
        
        skillInfo.Target.StatusEffectInstances.Add(instance);
    }
    
    public enum StatusEffectType
    {
        None
        //Poison
        //Sleep
        //etc
    }
}