using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

[InteractableNode(defaultNodeName = "Message Box")]
public class MessageBoxInteraction : Interaction
{
    [TextArea, Input] public string Text;
    [Input] public SoundInfo SoundInfo;
    [Input,AssetsOnly] public GameObject MessageBoxPrefab;

    private void Reset()
    {
        MessageBoxPrefab = GameSettings.Instance.DefaultMessageBox;
        SoundInfo = GameSettings.Instance.DefaultTypingSound;
    }

    //Chamar utility, chamar o digitador.
    public override IEnumerator PerformInteraction(Interactable source)
    {
        var text = GetInputValue<object>("Text", Text).ToString();
        var soundInfo = GetInputValue("SoundInfo", SoundInfo);
        text = SpecialTextMatcher.Match(source, text);
        var messageBoxPrefab = GetInputValue("MessageBoxPrefab", MessageBoxPrefab);

        var obj = Instantiate(messageBoxPrefab);
        var messageBox = obj.GetComponent<MessageBox>();
        
        if (soundInfo == null) messageBox.SetText(text);
        else messageBox.SetText(text,soundInfo);

        yield return new WaitUntil(() => messageBox.Dismissed);
        
        Destroy(obj);
    }
}
