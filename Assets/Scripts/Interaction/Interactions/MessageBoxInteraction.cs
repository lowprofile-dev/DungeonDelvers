using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

[InteractableNode(defaultNodeName = "Message Box")]
public class MessageBoxInteraction : Interaction
{
    [TextArea, Input] public string Text;
    [Input,AssetsOnly] public GameObject MessageBoxPrefab;
    
    //Chamar utility, chamar o digitador.
    public override IEnumerator PerformInteraction(Interactable source)
    {
        var text = GetInputValue<object>("Text", Text).ToString();
        text = SpecialTextMatcher.Match(source, text);
        var messageBoxPrefab = GetInputValue("MessageBoxPrefab", MessageBoxPrefab);

        var obj = Instantiate(messageBoxPrefab);
        var messageBox = obj.GetComponent<MessageBox>();
        messageBox.SetText(text);
        
        yield return new WaitUntil(() => messageBox.Dismissed);
        
        Destroy(obj);
    }
}
