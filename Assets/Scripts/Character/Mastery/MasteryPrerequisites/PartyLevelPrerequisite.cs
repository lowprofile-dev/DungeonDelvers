using XNode;

public class PartyLevelPrerequisite : Node, IMasteryPrerequisite
{
    public int MinPartyLevel;
    [Output] public MasteryNode LockedNode;

    public override object GetValue(NodePort port)
    {
        if (port.fieldName == "LockedNode") return this;
        return null;
    }

    public bool PrerequisiteAchieved(Character context)
    {
        return PlayerController.Instance.PartyLevel >= MinPartyLevel;
    }
}
