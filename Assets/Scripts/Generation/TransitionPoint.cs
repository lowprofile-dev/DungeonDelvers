using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class TransitionPoint : MonoBehaviour
{
    [ShowInInspector] public static TransitionPoint StartPoint { get; private set; }
    [ShowInInspector] public static TransitionPoint EndPoint { get; private set; }

    public Transform Point;
    public PlayerController.PlayerDirection EnterDirection;

    private void Awake()
    {
        switch (endType)
        {
            case PointType.Start:
            {
                if (StartPoint != null)
                {
                    Debug.LogError("Multiple Start Points.");
                    DestroyImmediate(StartPoint);
                }
                StartPoint = this;
                break;
            }
            case PointType.End:
            {
                if (EndPoint != null)
                {
                    Debug.LogError("Multiple End Points.");
                    DestroyImmediate(EndPoint);
                }

                EndPoint = this;
                break;
            }
        }
        if (GameController.Instance != null)
            TransitionPlayer();
    }

    private void TransitionPlayer()
    {
        if (GameController.Instance.Transition != endType) //end -> start, start -> end
        {
            var target = Point.transform.position;
            PlayerController.Instance.transform.position = new Vector3(target.x, target.y, PlayerController.Instance.transform.position.z);
            TrackPlayer.Instance.transform.position = new Vector3(target.x, target.y, TrackPlayer.Instance.transform.position.z);
            PlayerController.Instance.Direction = EnterDirection;
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