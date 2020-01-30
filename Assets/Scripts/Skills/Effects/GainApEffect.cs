public class GainApEffect : Effect
{
    public int ApAmount;

    public override EffectResult ExecuteEffect(SkillInfo skillInfo)
    {
        skillInfo.Target.CurrentEp += ApAmount;
        return new GainApEffectResult()
        {
            ApGained = ApAmount,
            skillInfo = skillInfo
        };
    }

    public class GainApEffectResult : EffectResult
    {
        public int ApGained;
    }
}