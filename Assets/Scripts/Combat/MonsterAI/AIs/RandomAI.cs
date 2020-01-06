using System.Linq;
using UnityEngine;

public class RandomAI : MonsterAI
{
    public override Turn BuildTurn(MonsterBattler battler)
    {
        var useableSkills = battler.Skills.Where(monsterSkill => monsterSkill.Evaluate(battler));

        if (useableSkills.Any() == false)
            return new Turn();
        
        var randomIndex = Random.Range(0, useableSkills.Count());
        var chosenSkill = useableSkills.ElementAt(randomIndex);

        var possibleTargets = BattleController.Instance.Party.Where(partyMember => !partyMember.Fainted);
        
        if (possibleTargets.Any() == false)
            return new Turn();
        
        //Ver quando tem multiplos alvos
        randomIndex = Random.Range(0, possibleTargets.Count());
        var chosenTargets = new[] {possibleTargets.ElementAt(randomIndex)};

        return new Turn
        {
            Skill = chosenSkill,
            Targets = chosenTargets
        };
    }
}