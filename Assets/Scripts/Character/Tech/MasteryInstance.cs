public class MasteryInstance
{
    public Mastery Mastery;
    public bool Acquired;

    public MasteryInstance() { }
    
    public MasteryInstance(Mastery mastery)
    {
        Mastery = mastery;
        Acquired = false;
    }
}