using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sirenix.Utilities;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/StatusEffect")]
public class StatusEffect : SerializableAsset
{
    public string StatusEffectName = "";
    [AssetIcon, Obsolete] public Sprite StatusEffectIcon;
    public List<StatusEffect> Cancels = new List<StatusEffect>();
    public StatusEffectType Type = StatusEffectType.None;
    public Sprite CustomIcon;
    public bool Hidden = false;
    public List<PassiveEffect> Effects = new List<PassiveEffect>();

    public async Task ApplyAsync(SkillInfo skillInfo,int turnDuration)
    {
        var instance = new StatusEffectInstance
        {
            Source = skillInfo.Source,
            Target = skillInfo.Target,
            StatusEffect = this,
            TurnDuration = turnDuration
        };
        
        await skillInfo.Target.ApplyStatusEffectAsync(instance);
    }

    public void Apply(SkillInfo skillInfo, int turnDuration)
    {
        var instance = new StatusEffectInstance
        {
            Source = skillInfo.Source,
            Target = skillInfo.Target,
            StatusEffect = this,
            TurnDuration = turnDuration
        };
        
        skillInfo.Target.ApplyStatusEffect(instance);
    }
    
    public enum StatusEffectType
    {
        None,
        Poison,
        Blind,
        PositiveStat,
        NegativeStat,
        Defend,
        Stun,
    }
}