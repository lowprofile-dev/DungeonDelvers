using System;
using E7.Introloop;
using Sirenix.OdinInspector;
using UnityEngine;

public class playbgm : MonoBehaviour
{
    public IntroloopAudio Audio;
    private IntroloopPlayer player;

    private void Awake()
    {
        player = GetComponent<IntroloopPlayer>();
    }

    private bool paused = false;
    
    [Button] private void Play()
    {
        if (paused) player.Resume(2f);
        else player.Play(Audio);
        paused = false;
    }

    [Button] private void Stop()
    {
        player.Pause(0.1f);
        paused = true;
    }

    [Button] private void Stop2()
    {
        player.Pause();
        paused = true;
    }
}
