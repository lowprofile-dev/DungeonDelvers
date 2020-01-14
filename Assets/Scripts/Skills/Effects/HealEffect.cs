public class HealEffect : Effect
{
    //Ver como vai ser essas formulas
    public int HealAmount;

    public override EffectResult ExecuteEffect(SkillInfo skillInfo)
    {
        skillInfo.Target.CurrentHp += HealAmount;
        return new HealEffectResult()
        {
            AmountHealed = HealAmount,
            skillInfo = skillInfo
        };
    }

    public class HealEffectResult : EffectResult
    {
        public int AmountHealed;
    }
}