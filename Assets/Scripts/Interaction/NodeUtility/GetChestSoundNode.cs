using XNode;

[InteractableNode(defaultNodeName = "Util/Get Chest Sound")]
public class GetChestSoundNode : Node
{
    [Input(ShowBackingValue.Never)] public ChestComponent ChestComponent;
    [Output] public SoundInfo SoundInfo;

    public override object GetValue(NodePort port)
    {
        if (port.fieldName == "SoundInfo")
        {
            var chestComponent = GetInputValue<ChestComponent>("ChestComponent");
            return null;
        }
        return null;
    }
}
