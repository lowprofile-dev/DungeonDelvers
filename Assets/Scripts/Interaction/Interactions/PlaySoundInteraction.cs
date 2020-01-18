using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PlaySoundInteraction : Interaction
{
    public AudioClip AudioClip;
    public AudioSource AudioSource;
    [Range(0,1)] public float Volume;
    public override void Run(Interactable source)
    {
        AudioSource.PlayOneShot(AudioClip,Volume);
    }

    public override IEnumerator Completion => new WaitWhile(() => AudioSource.isPlaying);
}

