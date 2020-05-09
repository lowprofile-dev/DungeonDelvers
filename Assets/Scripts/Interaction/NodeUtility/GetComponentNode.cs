using System;
using System.Collections;
using Sirenix.OdinInspector;
using SkredUtils;
using UnityEngine;
using XNode;

[InteractableNode(defaultNodeName = "Util/Get Component")]
public class GetComponentNode : Node
{
    public GetComponentMode Mode = GetComponentMode.Self;
    [ShowIf("modeIsChild")]public int ChildIndex = 0;
    public string typeName;
    [Output] public Component component;

    public override object GetValue(NodePort port)
    {
        if (port.fieldName == "component")
        {
            var owner = (graph as InteractionGraph)?.source;
            if (owner == null) return null;
            
            GameObject componentSource;
            switch (Mode)
            {
                case GetComponentMode.Self:
                    componentSource = owner.gameObject;
                    break;
                case GetComponentMode.Parent:
                    componentSource = owner.transform.parent.gameObject;
                    break;
                case GetComponentMode.Child:
                    componentSource = owner.transform.GetChild(0).gameObject;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (componentSource != null) return componentSource.GetComponent(typeName);
        }
        return null;
    }

    public enum GetComponentMode
    {
        Self,
        Parent,
        Child
    }
    
    #if UNITY_EDITOR
    private bool modeIsChild() => Mode == GetComponentMode.Child;
#endif
}
