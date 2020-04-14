
using System.Collections;

[InteractableNode(defaultNodeName = "Key/Set")]
public class SetKeyInteraction : Interaction
{
    [Input] public KeyType KeyType;
    [Input] public string Key;
    [Input] public int Value;
    
    public override IEnumerator PerformInteraction(Interactable source)
    {
        var keyType = GetInputValue("KeyType", KeyType);
        var key = GetInputValue("Key", Key);
        var value = GetInputValue("Value", Value);

        switch (keyType)
        {
            case KeyType.Global:
                GameController.SetGlobal(key,value);
                break;
            case KeyType.Local:
                source.SetLocal(key,Value);
                break;
            case KeyType.Instance:
                source.SetInstance(key,Value);
                break;
        }
        
        yield break;
    }
}

