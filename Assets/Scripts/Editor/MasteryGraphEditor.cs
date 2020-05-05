using System;
using Sirenix.Utilities;
using XNodeEditor;

[CustomNodeGraphEditor(typeof(MasteryGraph))]
public class MasteryGraphEditor : NodeGraphEditor
{
    public override string GetNodeMenuName(Type type) => IsRelevantType(type) ? base.GetNodeMenuName(type) : null;

    public bool IsRelevantType(Type type)
    {
        if (type.ImplementsOrInherits(typeof(MasteryNode))) return true;
        if (type.ImplementsOrInherits(typeof(IMasteryPrerequisite))) return true;
        return false;
    }
}