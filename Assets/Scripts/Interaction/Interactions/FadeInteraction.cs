using System.Collections;
using UnityEngine;

public class FadeInteraction : Interaction
{
    public Color FadeColor;
    public float Duration = 1f;
    public FadeMode fadeMode;
    
    public override void Run(Interactable source)
    {
        if (fadeMode == FadeMode.Out)
        {
            var fade = MainCanvas.Instance.FadeImage;
            fade.color = FadeColor;
            fade.enabled = true;
        }
    }

    private IEnumerator FadeInCoroutine()
    {
        var elapsedTime = 0f;
        var fade = MainCanvas.Instance.FadeImage;

        while (fade.enabled && fade.color.a > 0)
        {
            var frameTime = Time.deltaTime;
            elapsedTime += frameTime;
            var framePercentage = 1 - (elapsedTime / Duration);
            var newColor = new Color(FadeColor.r,FadeColor.g,FadeColor.b,framePercentage);
            fade.color = newColor;
            
            yield return null;
        }

        fade.enabled = false;
    }

    private IEnumerator FadeOutCoroutine()
    {
        var elapsedTime = 0f;
        var fade = MainCanvas.Instance.FadeImage;

        while (fade.enabled && fade.color.a < 1)
        {
            var frameTime = Time.deltaTime;
            elapsedTime += frameTime;
            var framePercentage = elapsedTime / Duration;
            var newColor = new Color(FadeColor.r,FadeColor.g,FadeColor.b,framePercentage);
            fade.color = newColor;
            
            yield return null;
        }
        
        fade.color = new Color(FadeColor.r,FadeColor.g,FadeColor.b, 1);
    }

    public override IEnumerator Completion
    {
        get
        {
            if (fadeMode == FadeMode.In)
                yield return FadeInCoroutine();
            else
                yield return FadeOutCoroutine();
        }
    }

    public enum FadeMode
    {
        Out,
        In
    }
}
