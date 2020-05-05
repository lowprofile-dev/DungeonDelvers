public interface ITargetSelector
{
    Battler[] GetTargets(Battler source, Skill skill);
}
