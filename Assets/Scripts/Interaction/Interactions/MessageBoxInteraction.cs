using System.Collections;
using UnityEngine;

[InteractableNode(defaultNodeName = "Message Box")]
public class MessageBoxInteraction : Interaction
{
    [TextArea, Input] public string Text;
    [Input] public GameObject MessageBoxPrefab;

    public override IEnumerator PerformInteraction(Interactable source)
    {
        var text = GetInputValue("Text", Text);
        var messageBoxPrefab = GetInputValue("MessageBoxPrefab", MessageBoxPrefab);

        var obj = Instantiate(messageBoxPrefab);
        var messageBox = obj.GetComponent<MessageBox>();
        messageBox.Text.text = text;
        
        yield return new WaitUntil(() => messageBox.Dismissed);
        
        Destroy(obj);
    }
}
