using System.Collections.Generic;
using System.Linq;
using XNode;

[InteractableNode(defaultNodeName = "String/Concat")]
public class StringConcatNode : Node
{
    [Input] public List<string> strings = new List<string>();
    [Output] public string output;

    public override object GetValue(NodePort port)
    {
        if (port.fieldName == "output")
        {
            var strs = GetInputValue("strings", strings);
            if (!strs.Any())
                return "";
            else
                return strs.Aggregate((s1, s2) => s1 + s1);
        }

        return null;
    }
}
