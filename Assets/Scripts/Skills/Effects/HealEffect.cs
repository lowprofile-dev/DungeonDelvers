public class HealEffect : Effect
{
    //Ver como vai ser essas formulas
    public int HealAmount;

    public override EffectResult ExecuteEffect(BattleController battle, Skill effectSource, IBattler source, IBattler target)
    {
        target.CurrentHp += HealAmount;
        return new HealEffectResult()
        {
            AmountHealed = HealAmount,
            Skill = effectSource,
            Source = source,
            Target = target
        };
    }

    public class HealEffectResult : EffectResult
    {
        public int AmountHealed;
    }
}