using System.Collections;
using UnityEngine;
using SkredUtils;

[InteractableNode(defaultNodeName = "Play Sound")]
public class PlaySoundInteraction : Interaction
{
    [Input] public SoundInfo SoundInfo;
    [Input] public AudioSource AudioSource;

    public override IEnumerator PerformInteraction(Interactable source)
    {
        var clip = GetInputValue("AudioClip", SoundInfo);
        var audioSource = GetInputValue("AudioSource", AudioSource);

        if (audioSource == null)
        {
            audioSource = source.gameObject.AddComponent<AudioSource>();
            audioSource.PlayOneShot(clip);
            yield return new WaitWhile(() => audioSource.isPlaying);
            Destroy(audioSource);
        }
        else
        {
            audioSource.PlayOneShot(clip);
            yield return new WaitWhile(() => audioSource.isPlaying);
        }
    }
}
