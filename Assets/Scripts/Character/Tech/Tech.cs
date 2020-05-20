using Sirenix.OdinInspector;
using SkredUtils;

[System.Serializable]
public abstract class Tech
{
    [ShowInInspector] public virtual string TechName { get; set; }
    [ShowInInspector] public virtual int RequiredLevel { get; set; }
    [ShowInInspector] public virtual int TechCost { get; set; }
    public abstract void Apply(Character context);
}

public class SkillTech : Tech
{
    public PlayerSkill Skill;

    public override void Apply(Character context)
    {
        context.Skills.Include(Skill);
    }
}

public class PassiveTech : Tech
{
    public Passive Passive;

    public override void Apply(Character context)
    {
        context.Passives.Include(Passive);
    }
}


