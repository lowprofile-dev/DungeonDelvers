using System;
using Sirenix.Utilities;
using XNodeEditor;

[CustomNodeGraphEditor(typeof(MasteryGraph))]
public class MasteryGraphEditor : NodeGraphEditor
{
    public override string GetNodeMenuName(Type type)
        => type.ImplementsOrInherits(typeof(MasteryNode)) ? base.GetNodeMenuName(type) : null;
}