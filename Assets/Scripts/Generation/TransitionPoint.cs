using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class TransitionPoint : MonoBehaviour
{
    public int ConnectionIndex;
    
    public Transform Point;
    public PlayerController.PlayerDirection EnterDirection;

    private void Start()
    {
        if (GameController.Instance != null)
            TransitionPlayer();
    }

    private void TransitionPlayer()
    {
        if (GameController.Instance.TransitionSource.HasValue && GameController.Instance.TransitionSource == ConnectionIndex) //end -> start, start -> end
        {
            var target = Point.transform.position;
            var playerTransform = PlayerController.Instance.transform;
            playerTransform.position = new Vector3(target.x, target.y, playerTransform.position.z);
            var trackTransform = TrackPlayer.Instance.transform;
            trackTransform.position = new Vector3(target.x, target.y, trackTransform.position.z);
            PlayerController.Instance.Direction = EnterDirection;
            GameController.Instance.TransitionSource = null;
        }
    }
}