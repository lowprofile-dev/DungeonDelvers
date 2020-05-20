using SkredUtils;

public class LearnSkillMasteryEffect : MasteryEffect
{
    public PlayerSkill Skill;

    public override void ApplyEffect(Character owner)
    {
        owner.Skills.Include(Skill);
    }
}