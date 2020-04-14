using System.Collections;
using UnityEngine;

[InteractableNode(defaultNodeName = "Play Sound")]
public class PlaySoundInteraction : Interaction
{
    [Input] public AudioClip AudioClip;
    [Input] public AudioSource AudioSource;
    [Range(0,1), Input] public float Volume;
    
    public override IEnumerator PerformInteraction(Interactable source)
    {
        var clip = GetInputValue("AudioClip", AudioClip);
        var audioSource = GetInputValue("AudioSource", AudioSource);
        var volume = GetInputValue("Volume", Volume);

        if (audioSource == null)
        {
            audioSource = source.gameObject.AddComponent<AudioSource>();
            audioSource.PlayOneShot(clip,volume);
            yield return new WaitWhile(() => audioSource.isPlaying);
            Destroy(audioSource);
        }
        else
        {
            audioSource.PlayOneShot(clip,volume);
            yield return new WaitWhile(() => audioSource.isPlaying);
        }
    }
}
