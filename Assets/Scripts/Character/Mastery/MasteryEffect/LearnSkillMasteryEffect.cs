using System.Collections.Generic;

public class LearnSkillMasteryEffect : MasteryEffect
{
    public List<PlayerSkill> Skills = new List<PlayerSkill>();
    public override void ApplyBonuses(int level, Character character)
    {
        character.Skills.AddRange(Skills);
    }
}