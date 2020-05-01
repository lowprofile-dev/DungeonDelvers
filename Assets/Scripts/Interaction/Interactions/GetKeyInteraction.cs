using System.Collections;
using XNode;

[InteractableNode(defaultNodeName = "Key/Get")]
public class GetKeyInteraction : Node
{
    [Input] public KeyType KeyType;
    //Hide if KeyType == Global
    [Input] public Interactable Interactable;
    [Input] public string Key;
    [Output] public int Value;

    public override object GetValue(NodePort port)
    {
        var keyType = GetInputValue("KeyType", KeyType);
        var interactable = GetInputValue("Interactable", Interactable);
        var key = GetInputValue("Key", Key);
        
        if (port.fieldName == "Value")
        {
            switch (keyType)
            {
                case KeyType.Global:
                    return GameController.GetGlobal(key);
                case KeyType.Local:
                    return interactable?.GetLocal(key);
                case KeyType.Instance:
                    return interactable?.GetInstance(key,0);
            }
        }
        return null;
    }
}
