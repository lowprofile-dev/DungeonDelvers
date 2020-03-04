using System.Linq;
using UnityEngine;

public class RandomAI : MonsterAI
{
    public override Turn BuildTurn(MonsterBattler battler)
    {
        var usableSkills = battler.Skills.Where(monsterSkill => monsterSkill.Evaluate(battler)).ToArray();

        if (usableSkills.Any() == false)
            return new Turn();
        
        var randomIndex = Random.Range(0, usableSkills.Count());
        var chosenSkill = usableSkills.ElementAt(randomIndex);

        var possibleTargets = BattleController.Instance.Party.Where(partyMember => !partyMember.Fainted).ToArray();
        
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