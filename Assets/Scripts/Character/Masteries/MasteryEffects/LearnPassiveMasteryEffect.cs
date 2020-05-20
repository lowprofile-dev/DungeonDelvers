using SkredUtils;

public class LearnPassiveMasteryEffect : MasteryEffect
{
    public Passive Passive;

    public override void ApplyEffect(Character owner)
    {
        owner.Passives.Include(Passive);
    }
}