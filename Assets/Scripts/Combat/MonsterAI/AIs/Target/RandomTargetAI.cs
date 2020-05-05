using SkredUtils;

public class RandomTargetAI : ITargetSelector
{
    public Battler[] GetTargets(Battler source, Skill skill)
    {
        var possibleTargets = BattleController.Instance.BuildPossibleTargets(source, skill.Target);
        return possibleTargets.Random();
    }
}
