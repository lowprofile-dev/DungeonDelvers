using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackTransform : MonoBehaviour
{
    public bool Track = true;
    public Vector2 Offset = new Vector2(0,0);
    public Transform Target;
    private new Rigidbody2D rigidbody2D;
    public float TrackSpeed;
    private void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (!Track || Target == null)
        {
            rigidbody2D.velocity = Vector2.zero;
            return;
        }
        
        var delta = Target.position - transform.position + (Vector3)Offset;
        rigidbody2D.velocity = delta * TrackSpeed;
    }
}