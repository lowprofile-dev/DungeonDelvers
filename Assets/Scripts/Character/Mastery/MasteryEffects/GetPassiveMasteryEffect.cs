public class GetPassiveMasteryEffect : MasteryEffect
{
    public Passive Passive;
    
    public override void ApplyEffect(Character character, int level)
    {
        if (level > 0) character.Passives.Add(Passive);
    }
}
