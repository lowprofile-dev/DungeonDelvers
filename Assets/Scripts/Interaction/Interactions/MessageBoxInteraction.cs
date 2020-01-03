using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageBoxInteraction : Interaction
{
    [TextArea] public string Text;
    public GameObject MessageBoxPrefab;
    private MessageBox MessageBox;

    public override void Run(Interactable source)
    {
        var obj = Object.Instantiate(MessageBoxPrefab);
        MessageBox = obj.GetComponent<MessageBox>();
        MessageBox.Text.text = Text;
    }

    public override void Cleanup()
    {
        Object.Destroy(MessageBox.gameObject);
    }

    public override IEnumerator Completion => new WaitUntil(() => MessageBox.Dismissed);
}