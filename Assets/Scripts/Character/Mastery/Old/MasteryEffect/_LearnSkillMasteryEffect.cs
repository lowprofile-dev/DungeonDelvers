using System.Collections.Generic;


public class _LearnSkillMasteryEffect : _MasteryEffect
{
    public List<PlayerSkill> Skills = new List<PlayerSkill>();

    public override void ApplyBonuses(int level, Character character)
    {
        character.Skills.AddRange(Skills);
    }
}