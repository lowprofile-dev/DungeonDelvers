using System.Linq;
using XNode;

public class PartialLevelPrerequisite : Node, IMasteryPrerequisite
{
    [Input(ShowBackingValue.Never)] public MasteryNode PrerequisiteNode;
    public int MinLevel;
    [Output] public MasteryNode LockedNode;
    
    public override object GetValue(NodePort port)
    {
        if (port.fieldName == "LockedNode") return this;
        return null;
    }
    
    public bool PrerequisiteAchieved(Character context)
    {
        var preReq = GetInputValue<MasteryNode>("PrerequisiteNode");
        if (preReq == null)
            return true;

        var preReqInstance = context.MasteryInstances.FirstOrDefault(mI => mI.Node == preReq);
        if (preReqInstance == null)
            return false;

        return preReqInstance.Level >= MinLevel;
    }
}
