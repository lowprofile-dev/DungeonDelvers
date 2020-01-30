using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;

//POR ENQUANTO
public class StatusEffect : IPassiveEffectSource
{
    public int TurnDuration = 0;
    public string StatusEffectName = "";
    public string GetName => StatusEffectName;
    public List<PassiveEffect> Effects = new List<PassiveEffect>();
    public List<PassiveEffect> GetEffects => Effects;

    public void Apply(IBattler battler)
    {
        var instance = new StatusEffect
        {
            TurnDuration = TurnDuration + BattleController.Instance.CurrentTurn,
            StatusEffectName = StatusEffectName,
            Effects = Effects
        };

        //Refazer isso pra copiar cada efeito também (?)
        instance.Effects.ForEach(effect => effect.PassiveSource = instance);
        
        battler.StatusEffects.Add(instance);
    }
}