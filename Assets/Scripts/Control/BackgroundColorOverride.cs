using System;
using UnityEngine;

public class BackgroundColorOverride : MonoBehaviour
{
    public Color BackgroundColor;


    private void Start()
    {
        TrackPlayer.Instance.camera.backgroundColor = BackgroundColor;
    }
}