using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionInteraction : Interaction
{
    public TransitionPoint.PointType Direction;
    
    public override void Run(Interactable source)
    {
        //tela de carregar futuramente
        var index = Direction == TransitionPoint.PointType.Start
            ? MapSettings.Instance.previousSceneIndex
            : MapSettings.Instance.nextSceneIndex;

        GameController.Instance.Transition = Direction;
        
        Debug.Log($"Transitioning to index {index}");
        
        SceneManager.LoadScene(index);
    }

    public override IEnumerator Completion => null;
}