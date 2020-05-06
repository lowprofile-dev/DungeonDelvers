using System;
using System.Linq;
using SkredUtils;

public class RandomSkillAI : ISkillSelector
{
    public RandomType randomType = RandomType.True;
    
    public Skill GetSkill(Battler source)
    {
        var skills = source.SkillList.Where(skill => skill.ApCost <= source.CurrentAp).ToArray();
        
        if (!skills.Any()) return null;
        if (skills.Length == 1) return skills[0];
        
        switch (randomType)
        {
            case RandomType.True:
            {
                return skills.Random();
            }
            case RandomType.ApWeighted:
            {
                return skills.WeightedRandom(skill => skill.ApCost);
            }
            case RandomType.ApFavoured:
            {
                int highestCost = int.MinValue;
                foreach (var skill in skills)
                {
                    if (skill.ApCost > highestCost) highestCost = skill.ApCost;
                }
                var skillsWithHighestCost = skills.Where(skill => skill.ApCost == highestCost);
                return skillsWithHighestCost.Random();
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
        
    }

    [Serializable]
    public enum RandomType
    {
        True,
        ApWeighted,
        ApFavoured
    }
}
