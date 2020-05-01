using System;
using XNode;

[InteractableNode(defaultNodeName = "Comparison")]
public class ComparisonNode : Node
{
    [Input] public int variable1;
    [Input] public int variable2;
    [Input] public Comparison comparison;
    [Output] public bool result;

    public enum Comparison
    {
        EqualTo,
        UnequalTo,
        GreaterThan,
        GreaterThanOrEqualTo,
        LesserThan,
        LesserThanOrEqualTo,
    }
    
    public override object GetValue(NodePort port)
    {
        if (port.fieldName == "result")
        {
            var var1 = GetInputValue("variable1", variable1);
            var var2 = GetInputValue("variable2", variable2);
            switch (comparison)
            {
                case Comparison.EqualTo:
                    return var1 == var2;
                case Comparison.UnequalTo:
                    return var1 != var2;
                case Comparison.GreaterThan:
                    return var1 > var2;
                case Comparison.GreaterThanOrEqualTo:
                    return var1 >= var2;
                case Comparison.LesserThan:
                    return var1 < var2;
                case Comparison.LesserThanOrEqualTo:
                    return var1 <= var2;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        } 
        return null;
    }
}
