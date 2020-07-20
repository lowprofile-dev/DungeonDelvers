using Sirenix.OdinInspector;
using SkredUtils;

[System.Serializable]
public abstract class Mastery
{
    [ShowInInspector] public virtual string MasteryName { get; set; }
    [ShowInInspector] public virtual int RequiredLevel { get; set; }
    [ShowInInspector] public virtual int TechCost { get; set; }
    public abstract void Apply(Character context);
}

public class SkillMastery : Mastery
{
    public PlayerSkill Skill;

    public override void Apply(Character context)
    {
        context.Skills.Include(Skill);
    }
}

public class PassiveMastery : Mastery
{
    public Passive Passive;

    public override void Apply(Character context)
    {
        context.Passives.Include(Passive);
    }
}


