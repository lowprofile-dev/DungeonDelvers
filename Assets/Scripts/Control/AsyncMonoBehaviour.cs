using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using SkredUtils;
using UnityEngine;

public class AsyncMonoBehaviour : SerializedMonoBehaviour
{
    private Queue<Action> QueuedActions = new Queue<Action>();

    public async Task QueueActionAndAwait(Action action)
    {
        var completed = false;
        var fullAction = (action += () => completed = true);

        QueueAction(fullAction);

        while (!completed)
        {
            await Task.Delay(5);
        }
    }

    public async Task PlayCoroutine(IEnumerator coroutine, MonoBehaviour target = null)
    {
        var completion = new Ref<bool>(false);
        Action action = () =>
        {
            if (target == null)
                target = this;
            target.StartCoroutine(AwaitCoroutineCompletion(coroutine, completion));
        };

        QueueAction(action);

        while (completion.Instance == false)
            await Task.Delay(5);
    }

    private IEnumerator AwaitCoroutineCompletion(IEnumerator coroutine, Ref<bool> completed)
    {
        yield return coroutine;
        completed.Instance = true;
    }

    public void QueueAction(Action action)
    {
        QueuedActions.Enqueue(action);
    }

    protected virtual void LateUpdate()
    {
        lock (QueuedActions)
        {
            try
            {
                while (QueuedActions.Any())
                {
                    var action = QueuedActions.Dequeue();
                    action.Invoke();
                }
            } catch (InvalidOperationException ioe){}
        }
    }
}