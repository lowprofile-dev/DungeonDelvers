public class GainApEffect : Effect
{
    public int ApAmount;

    public override EffectResult ExecuteEffect(SkillInfo skillInfo)
    {
        skillInfo.Target.CurrentAp += ApAmount;
        return new GainApEffectResult()
        {
            ApGained = ApAmount,
            skillInfo = skillInfo
        };
    }

    public override object Clone()
    {
        return new GainApEffect
        {
            ElementOverride = ElementOverride,
            ApAmount = ApAmount
        };
    }

    public class GainApEffectResult : EffectResult
    {
        public int ApGained;
    }
}