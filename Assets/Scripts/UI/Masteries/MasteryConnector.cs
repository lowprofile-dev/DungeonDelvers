using System;
using System.Collections;
using DG.Tweening;
using MasteriesV3;
using Sirenix.OdinInspector;
using UnityEngine;

public class MasteryConnector : MonoBehaviour
{
    public Mastery From;
    public Mastery To;
    public Animator Animator;

    private void Awake()
    {
        if (Animator == null) Animator = GetComponent<Animator>();
    }

    public void Setup(Mastery from, Mastery to)
    {
        From = from;
        To = to;
        RegeneratePosition();
    }
    
    public void SetLocked()
    {
        Animator.Play("Locked");
    }

    public void SetUnlocked()
    {
        Animator.Play("Unlocked");
    }

    public IEnumerator Unlock(float speed = 1f)
    {
        Animator.speed = speed;
        Animator.Play("Unlock");
        yield return new WaitForEndOfFrame();
        yield return new WaitWhile(() => Animator.GetCurrentAnimatorStateInfo(0).IsName("Unlock"));
    }
    
    [Button]
    private void RegeneratePosition()
    {
        var rect = (RectTransform) transform;

        var fromPos = From.transform.localPosition;
        var toPos = To.transform.localPosition;
        
        var position = (fromPos + toPos)/2;
        
        rect.localPosition = position;

        var dirVec = (toPos - fromPos);
        //var dirSign = (toPos.y < fromPos.y) ? -1.0f : 1.0f;
        var angle = Vector3.Angle(Vector2.up, dirVec) * -1;// * dirSign;

        var totalLength = dirVec.magnitude;
        var scale = totalLength / 180f;
        rect.localScale = new Vector3(1, scale, 1);
        rect.localRotation = Quaternion.Euler(0,0,angle+180);
    }
}
