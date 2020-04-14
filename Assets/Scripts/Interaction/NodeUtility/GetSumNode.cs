using XNode;

[InteractableNode(defaultNodeName = "Math/Sum")]
public class GetSumNode : Node
{
    [Input] public int a;
    [Input] public int b;
    [Output] public int result;

    public override object GetValue(NodePort port)
    {
        if (port.fieldName == "result")
            return GetInputValue<int>("a", a) + GetInputValue<int>("b", b);
        return null;
    }
}
