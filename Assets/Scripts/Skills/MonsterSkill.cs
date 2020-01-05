using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/MonsterSkill")]
public class MonsterSkill : Skill
{
    public List<ISkillCondition> Conditions = new List<ISkillCondition>();

    public bool Evaluate(MonsterBattler battler)
    {
        return Conditions.All(condition => condition.Evalute(battler, this));
    }
}