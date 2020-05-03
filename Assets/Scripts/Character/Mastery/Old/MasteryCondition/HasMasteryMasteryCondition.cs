public class HasMasteryMasteryCondition : MasteryCondition
{
    public _Mastery Mastery;
    public int RequiredLevel;

    public override bool Achieved(MasteryGroup group)
    {
        var hasMastery = group.Masteries.ContainsKey(Mastery);

        if (!hasMastery)
            return false;

        var instance = group.Masteries[Mastery];

        return instance.CurrentLevel >= RequiredLevel;
    }
}