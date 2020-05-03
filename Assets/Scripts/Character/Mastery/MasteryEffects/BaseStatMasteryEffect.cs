using System;

[Serializable]
public class BaseStatMasteryEffect : MasteryEffect
{
    public Stats Stats;
    
    public override void ApplyEffect(Character character, int level)
    {
        character.BaseStats += Stats * level;
    }
}
