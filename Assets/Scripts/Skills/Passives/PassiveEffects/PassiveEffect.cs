public abstract class PassiveEffect
{
    public int Priority = 0;
}

public struct PassiveEffectInfo
{
    public string PassiveEffectSourceName;
    public Battler Source;
    public Battler Target;
}
