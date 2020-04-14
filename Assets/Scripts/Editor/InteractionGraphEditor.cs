using System;
using XNodeEditor;

[CustomNodeGraphEditor(typeof(InteractionGraph))]
public class InteractionGraphEditor : NodeGraphEditor
{
    public override string GetNodeMenuName(Type type)
    {
        var isInteractableNode = Attribute.GetCustomAttribute(type, typeof(InteractableNodeAttribute));
        if (isInteractableNode == null)
            return null;

        var nodeAttributeName = (isInteractableNode as InteractableNodeAttribute)?.defaultNodeName;
        return string.IsNullOrEmpty(nodeAttributeName) ? base.GetNodeMenuName(type) : nodeAttributeName;
    }
}
