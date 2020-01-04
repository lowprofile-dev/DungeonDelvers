using System.Linq;
using UnityEngine;

public class RandomAI : MonsterAI
{
    public override Skill ChooseSkill(MonsterBattler battler)
    {
        var useableSkills = battler.Skills.Where(monsterSkill => monsterSkill.Evaluate(battler));

        if (useableSkills.Any() == false)
            return null;
        
        var randomIndex = Random.Range(0, useableSkills.Count());
        return useableSkills.ElementAt(randomIndex);
    }
}