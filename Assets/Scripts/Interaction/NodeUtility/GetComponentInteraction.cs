using System.Collections;
using SkredUtils;
using UnityEngine;
using XNode;

[InteractableNode(defaultNodeName = "Util/Get Component")]
public class GetComponentInteraction : Node
{
    public string typeName;
    [Output] public Component component;

    public override object GetValue(NodePort port)
    {
        if (port.fieldName == "component")
        {
            var owner = (graph as InteractionGraph)?.source;
            if (owner != null) return owner.GetComponent(typeName);
        }
        return null;
    }
}
