using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;

public class StatusEffect : IPassiveEffectSource
{
    public int TurnDuration = 0;
    public string StatusEffectName = "";
    public string GetName => StatusEffectName;
    public StatusEffectType Type = StatusEffectType.None;
    public bool Hidden = false;
    public List<PassiveEffect> Effects = new List<PassiveEffect>();
    public List<PassiveEffect> GetEffects => Effects;

    public void Apply(SkillInfo skillInfo)
    {
        var instance = new StatusEffect
        {
            TurnDuration = TurnDuration + BattleController.Instance.CurrentTurn,
            StatusEffectName = StatusEffectName,
            Effects = Effects.Select(effect => effect.GetInstance()).ToList()
        };
        
        instance.Effects.ForEach(passiveEffect =>
        {
            passiveEffect.PassiveSource = this;

            if (passiveEffect is IHasSource ihs)
            {
                ihs.Source = skillInfo.Source;
            }

            if (passiveEffect is IHasTarget iht)
            {
                iht.Target = skillInfo.Target;
            }
        });

        skillInfo.Target.StatusEffects.Add(instance);
    }

    // private void ApplyEffect(PassiveEffect passiveEffect)
    // {
    //     passiveEffect.PassiveSource = this;
    //
    //     if (passiveEffect is IHasSource ihs)
    //     {
    //         
    //     }
    // }

    public enum StatusEffectType
    {
        None
    }
}