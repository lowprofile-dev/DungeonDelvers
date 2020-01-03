public class ValueCondition : ICondition
{
    public ValueScope Scope = ValueScope.Global;
    public string Key = "";
    public Comparison Expected = Comparison.EqualTo;
    public int Value = 0;
    
    public bool ConditionReached(Interactable source)
    {
        int value;
        if (Scope == ValueScope.Global)
            value = GameController.GetGlobal(Key);
        else
            value = source.GetLocal(Key);

        switch (Expected)
        {
            case Comparison.EqualTo:
                return value == Value;
            case Comparison.UnequalTo:
                return value != Value;
            case Comparison.GreaterThan:
                return value > Value;
            case Comparison.LessThan:
                return value < Value;
            default:
                return false;
        }
    }

    public enum ValueScope
    {
        Local,
        Global
    }

    public enum Comparison
    {
        EqualTo,
        UnequalTo,
        GreaterThan,
        LessThan
    }
}