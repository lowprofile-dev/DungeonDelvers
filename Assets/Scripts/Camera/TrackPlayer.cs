using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackPlayer : MonoBehaviour, IMapSettingsListener
{
    public static TrackPlayer Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
            return;
        }

        Instance = this;
    }

    public Camera camera;
    public bool Track = true;
    public Vector2 Offset = new Vector2(0, 0);
    public Transform Player;
    private new Rigidbody2D rigidbody2D;
    public float TrackSpeed;

    private void Start()
    {
        DontDestroyOnLoad(this);
        rigidbody2D = GetComponent<Rigidbody2D>();
        Player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void FixedUpdate()
    {
        if (!Track || Player == null)
        {
            rigidbody2D.velocity = Vector2.zero;
            return;
        }

        var delta = Player.position - transform.position + (Vector3)Offset;
        rigidbody2D.velocity = delta * TrackSpeed;
    }

    public void ApplyMapSettings(MapSettings mapSettings)
    {
        camera.backgroundColor = mapSettings.OverrideBackgroundColor;
    }
}