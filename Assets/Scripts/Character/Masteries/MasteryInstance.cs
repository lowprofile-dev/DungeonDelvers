using System.Linq;

public class MasteryInstance
{
    public int Id;
    public Mastery.MasteryStatus Status;

    public MasteryInstance()
    {
    }

    public MasteryInstance(Mastery mastery)
    {
        Id = mastery.Id;
        Status = mastery.AutoLearned
            ? Mastery.MasteryStatus.Learned
            : mastery.MasteryPrerequisites.Any()
                ? Mastery.MasteryStatus.Locked
                : Mastery.MasteryStatus.Unlocked;
    }
}