using System.Collections.Generic;

public interface IPassiveEffectSource
{
    string GetName { get; }
    List<PassiveEffect> GetEffects { get; }
}


//ver como vai ficar
public interface IHasTarget
{
    IBattler Target { get; set; }
}

public interface IHasSource
{
    IBattler Source { get; set; }
}