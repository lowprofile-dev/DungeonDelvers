using System;
using System.Collections;
using SkredUtils;
using UnityEngine;
using UnityEngine.Events;

public abstract class UIPanel : MonoBehaviour
{
    public UnityEvent OnOpen;
    public UnityEvent OnClose;

    private bool menuClosed;

    protected virtual void Awake()
    {
        gameObject.SetActive(false);
    }

    public virtual IEnumerator Open()
    {
        gameObject.SetActive(true);
        OnOpen.Invoke();
        return WaitUntilClosed();
    }

    public virtual void Close()
    {
        gameObject.SetActive(false);
        OnClose.Invoke();
    }
    
    private IEnumerator WaitUntilClosed()
    {
        while (!menuClosed)
            yield return null;
    }
}
