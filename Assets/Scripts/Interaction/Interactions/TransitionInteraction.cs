using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionInteraction : Interaction
{
    public int sceneTargetIndex;
    
    public override void Run(Interactable source)
    {
        //tela de carregar futuramente
        
        GameController.Instance.TransitionSource = SceneManager.GetActiveScene().buildIndex;

        SceneManager.LoadScene(sceneTargetIndex);
    }

    public override IEnumerator Completion => null;
}