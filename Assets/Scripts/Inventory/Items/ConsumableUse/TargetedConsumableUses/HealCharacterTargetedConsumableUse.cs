using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealCharacterTargetedConsumableUse : TargetedConsumableUse
{
    public int HealAmount;

    public override IEnumerator ApplyToCharacter(Character character)
    {
        yield return null;
        character.CurrentHp += HealAmount;
    }
}
