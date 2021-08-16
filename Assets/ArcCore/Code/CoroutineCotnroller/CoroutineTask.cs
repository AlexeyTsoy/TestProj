using System;
using System.Collections;
using UnityEngine;
public enum CoroutineState
{
    Ready,
    Running,
    Paused,
    Finished
}

public class CoroutineTask
{
    public CoroutineState State { get; private set; }

    private readonly IEnumerator _routine;

    private readonly string _id;

    private readonly Action<string> _callback;

    public CoroutineTask(IEnumerator routine, string id, Action<string> callback)
    {
        _routine = routine;
        _id = id;
        _callback = callback;
        State = CoroutineState.Ready;
    }

    public IEnumerator Start(Action onComplete)
    {
        if (State != CoroutineState.Ready)
        {
            Debug.LogError($"Unable to start coroutine in state: {State}");
            yield break;
        }

        State = CoroutineState.Running;
        while (_routine.MoveNext())
        {
            yield return _routine.Current;
            while (State == CoroutineState.Paused)
            {
                yield return null;
            }
            if (State == CoroutineState.Finished)
            {
                yield break;
            }
        }

        State = CoroutineState.Finished;
        _callback?.Invoke(_id);
        onComplete?.Invoke();
    }

    public void Stop()
    {
        if (State == CoroutineState.Finished)
        {
            return;
        }
        _callback?.Invoke(_id);

        State = CoroutineState.Finished;
    }

    public void Pause()
    {
        if (State != CoroutineState.Running)
        {
            return;
        }

        State = CoroutineState.Paused;
    }

    public void Resume()
    {
        if (State != CoroutineState.Paused)
        {
            return;
        }

        State = CoroutineState.Running;
    }

}
