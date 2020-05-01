
using System;
using System.Collections;

[InteractableNode(defaultNodeName = "Key/Set")]
public class SetKeyInteraction : Interaction
{
    [Input] public KeyType KeyType;
    [Input] public string Key;
    [Input] public int Value;
    public SetType setType;
    
    public enum SetType
    {
        Set,
        Add,
        Subtract
    }
    
    public override IEnumerator PerformInteraction(Interactable source)
    {
        var keyType = GetInputValue("KeyType", KeyType);
        var key = GetInputValue("Key", Key);
        var value = GetInputValue("Value", Value);

        switch (keyType)
        {
            case KeyType.Global:
                switch (setType)
                {
                    case SetType.Set:
                        GameController.SetGlobal(key,value);
                        break;
                    case SetType.Add:
                        GameController.SetGlobal(key,GameController.GetGlobal(key)+value);
                        break;
                    case SetType.Subtract:
                        GameController.SetGlobal(key,GameController.GetGlobal(key)-value);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                break;
            case KeyType.Local:
                switch (setType)
                {
                    case SetType.Set:
                        source.SetLocal(key,value);
                        break;
                    case SetType.Add:
                        source.SetLocal(key,source.GetLocal(key)+value);
                        break;
                    case SetType.Subtract:
                        source.SetLocal(key,source.GetLocal(key)-value);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                break;
            case KeyType.Instance:
                switch (setType)
                {
                    case SetType.Set:
                        source.SetInstance(key,value);
                        break;
                    case SetType.Add:
                        source.SetInstance(key,(int)source.GetInstance(key,0)+value);
                        break;
                    case SetType.Subtract:
                        source.SetInstance(key,(int)source.GetInstance(key,0)-value);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                break;
        }
        yield break;
    }
}

