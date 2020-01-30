using System.Collections.Generic;

public interface IPassiveEffectSource
{
    string GetName { get; }
    List<PassiveEffect> GetEffects { get; }
}