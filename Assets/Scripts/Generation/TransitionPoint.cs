using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class TransitionPoint : MonoBehaviour
{
    [ShowInInspector, HideInEditorMode] public static TransitionPoint StartPoint { get; private set; }
    [ShowInInspector, HideInEditorMode] public static TransitionPoint EndPoint { get; private set; }

    public Transform Point;
    public PlayerController.PlayerDirection EnterDirection;

    private void Awake()
    {
        switch (endType)
        {
            case PointType.Start:
            {
                StartPoint = this;
                break;
            }
            case PointType.End:
            {
                EndPoint = this;
                break;
            }
        }
    }

    private void Start()
    {
        if (GameController.Instance != null)
            TransitionPlayer();
    }

    private void TransitionPlayer()
    {
        if (GameController.Instance.Transition.HasValue && GameController.Instance.Transition != endType) //end -> start, start -> end
        {
            var target = Point.transform.position;
            var playerTransform = PlayerController.Instance.transform;
            playerTransform.position = new Vector3(target.x, target.y, playerTransform.position.z);
            var trackTransform = TrackPlayer.Instance.transform;
            trackTransform.position = new Vector3(target.x, target.y, trackTransform.position.z);
            PlayerController.Instance.Direction = EnterDirection;
            GameController.Instance.Transition = null;
        }
    }
    
    public PointType endType;

    [Serializable]
    public enum PointType
    {
        Start,
        End
    }
}