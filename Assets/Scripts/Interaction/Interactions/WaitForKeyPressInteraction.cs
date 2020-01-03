using System.Collections;
using UnityEngine;

public class WaitForKeyPressInteraction : Interaction
{
    private bool keepWaiting = true;
    public KeyCode Key;
    
    public override void Run(Interactable source)
    {
        keepWaiting = true;
        Debug.Log("Esperando");
        source.StartCoroutine(WaitForKeyPress());
    }

    public override IEnumerator Completion
    {
        get { return new WaitUntil(() => !keepWaiting);}
    }

    IEnumerator WaitForKeyPress()
    {
        while (!Input.GetKeyDown(Key))
            yield return null;

        Debug.Log("foi");
        
        keepWaiting = false;
    }
}