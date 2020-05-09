using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[InteractableNode(defaultNodeName = "Fade Interaction")]
public class FadeInteraction : Interaction
{
    [Input] public Color fadeColor = Color.black;
    [Input] public float duration = 1f;
    [Input] public FadeMode fadeMode = FadeMode.Out;
    
    public override IEnumerator PerformInteraction(Interactable source)
    {
        var mode = GetInputValue("fadeMode", fadeMode);
        var color = GetInputValue("fadeColor", fadeColor);
        var fadeDuration = GetInputValue("duration", duration);
        Image fade;

        try
        {
            fade = MainCanvas.Instance.FadeImage;
        }
        catch (Exception e)
        {
            throw e;
        }
        
        if (mode == FadeMode.Out)
        {
            var elapsedTime = 0f;
            fade.enabled = true;

            while (fade.enabled && elapsedTime < fadeDuration)
            {
                var frameTime = Time.deltaTime;
                elapsedTime += frameTime;
                var framePercentage = elapsedTime / fadeDuration;
                var newColor = new Color(color.r,color.g,color.b,framePercentage);
                fade.color = newColor;
            
                yield return null;
            }
        
            fade.color = new Color(color.r,color.g,color.b, 1);
        }
        else if (mode == FadeMode.In)
        {
            fade.color = color;

            var elapsedTime = 0f;

            while (fade.enabled && elapsedTime < fadeDuration)
            {
                var frameTime = Time.deltaTime;
                elapsedTime += frameTime;
                var framePercentage = 1 - (elapsedTime / fadeDuration);
                var newColor = new Color(color.r,color.g,color.b,framePercentage);
                fade.color = newColor;
            
                yield return null;
            }

            fade.enabled = false;
        }
    }

    [Serializable]
    public enum FadeMode
    {
        Out,
        In
    }
}
