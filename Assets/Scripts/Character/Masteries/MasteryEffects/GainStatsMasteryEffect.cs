public class GainStatsMasteryEffect : MasteryEffect
{
    public Stats Stats;

    public override void ApplyEffect(Character owner)
    {
        owner.BaseStats += Stats;
    }
}