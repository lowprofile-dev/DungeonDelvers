using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class playanim : MonoBehaviour
{
    public string animName;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z)) PlayAnim();
    }

    [Button] private void PlayAnim()
    {
        var anim = GetComponent<Animator>();
        anim.Play(animName);
    }
}
