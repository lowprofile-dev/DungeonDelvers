public class _BaseStatMasteryEffect : _MasteryEffect
{
    public Stats Stats;

    public override void ApplyBonuses(int level, Character character)
    {
        var totalStats = new Stats();

        for (int i = 0; i < level; i++)
        {
            totalStats += Stats;
        }

        character.BaseStats += totalStats;
    }
}