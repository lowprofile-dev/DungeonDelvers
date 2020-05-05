using System.Linq;
using SkredUtils;

public class RandomSkillAI : ISkillSelector
{
    public Skill GetSkill(Battler source)
    {
        var skills = source.SkillList.Where(skill => skill.ApCost <= source.CurrentAp);
        return skills.Random();
    }
}
