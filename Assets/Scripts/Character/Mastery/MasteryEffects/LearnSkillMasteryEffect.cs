public class LearnSkillMasteryEffect : MasteryEffect
{
    public PlayerSkill Skill;
    
    public override void ApplyEffect(Character character, int level)
    {
        if (level > 0) character.Skills.Add(Skill);
    }
}
