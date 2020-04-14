using System.Collections;
using UnityEngine.SceneManagement;

[InteractableNode(defaultNodeName = "Transition")]
public class TransitionInteraction : Interaction
{
    [Input] public int Index;
    
    public override IEnumerator PerformInteraction(Interactable source)
    {
        var index = GetInputValue("Index", Index);
        GameController.Instance.TransitionSource = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(index);
        
        yield break;
    }
}
