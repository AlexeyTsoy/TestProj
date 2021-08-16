using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class CoroutineController
{
    private static readonly CoroutineManager _coroutineManager;
    private static readonly CoroutineManager _freeCoroutineManager;
    private static readonly Dictionary<string, CoroutineTask> _coroutines;
    private static readonly Dictionary<string, Queue<IEnumerator>> _queuedCoroutines;

    private static readonly Dictionary<string, List<IEnumerator>> _packedCoroutines;

    static CoroutineController()
    {
        _coroutines = new Dictionary<string, CoroutineTask>();
        _queuedCoroutines = new Dictionary<string, Queue<IEnumerator>>();
        _packedCoroutines = new Dictionary<string, List<IEnumerator>>();

#if UNITY_EDITOR
        if (!EditorApplication.isPlayingOrWillChangePlaymode || !EditorApplication.isPlaying) return;
#endif
        _coroutineManager = new GameObject("CoroutineManager").AddComponent<CoroutineManager>();
        _freeCoroutineManager = new GameObject("FreeCoroutineManager").AddComponent<CoroutineManager>();
    }

    public static CoroutineTask WaitForSeconds(Action method, float time, string id = "", Action onComplete = null)
    {
        if (_coroutineManager == null) return null;

        var coroutineTask = new CoroutineTask(WaitForSecondsRoutine(time, method), id, Remove);

        if (!string.IsNullOrEmpty(id))
        {
            if (_coroutines.TryGetValue(id, out var result))
            {
                result?.Stop();
            }

            _coroutines[id] = coroutineTask;
        }

        _coroutineManager.StartCoroutine(coroutineTask.Start(onComplete));
        return coroutineTask;
    }    
    
    public static CoroutineTask WaitForRealSeconds(Action method, float time, string id = "", Action onComplete = null)
    {
        if (_coroutineManager == null) return null;

        var coroutineTask = new CoroutineTask(WaitForRealSecondsRoutine(time, method), id, Remove);

        if (!string.IsNullOrEmpty(id))
        {
            if (_coroutines.TryGetValue(id, out var result))
            {
                result?.Stop();
            }

            _coroutines[id] = coroutineTask;
        }

        _coroutineManager.StartCoroutine(coroutineTask.Start(onComplete));
        return coroutineTask;
    }

    public static CoroutineTask WaitUntil(Action method, Func<bool> predicate, string id = "", Action onComplete = null)
    {
        if (_coroutineManager == null) return null;

        var coroutineTask = new CoroutineTask(WaitUntilRoutine(predicate, method), id, Remove);

        if (!string.IsNullOrEmpty(id))
        {
            if (_coroutines.TryGetValue(id, out var result))
            {
                result?.Stop();
            }

            _coroutines[id] = coroutineTask;
        }

        _coroutineManager.StartCoroutine(coroutineTask.Start(onComplete));
        return coroutineTask;
    }
    
    public static CoroutineTask WaitWhile(Action method, Func<bool> predicate, string id = "", Action onComplete = null)
    {
        if (_coroutineManager == null) return null;

        var coroutineTask = new CoroutineTask(WaitWhileRoutine(predicate, method), id, Remove);

        if (!string.IsNullOrEmpty(id))
        {
            if (_coroutines.TryGetValue(id, out var result))
            {
                result?.Stop();
            }

            _coroutines[id] = coroutineTask;
        }

        _coroutineManager.StartCoroutine(coroutineTask.Start(onComplete));
        return coroutineTask;
    }

    public static CoroutineTask WaitEndOfFrame(Action method, string id = "", Action onComplete = null)
    {
        if (_coroutineManager == null) return null;

        var coroutineTask = new CoroutineTask(WaitEndOfFrameRoutine(method), id, Remove);

        if (!string.IsNullOrEmpty(id))
        {
            if (_coroutines.TryGetValue(id, out var result))
            {
                result?.Stop();
            }

            _coroutines[id] = coroutineTask;
        }

        _coroutineManager.StartCoroutine(coroutineTask.Start(onComplete));
        return coroutineTask;
    }

    private static IEnumerator WaitForSecondsRoutine(float time, Action method)
    {
        yield return new WaitForSeconds(time);
        method?.Invoke();
    }  
    
    private static IEnumerator WaitForRealSecondsRoutine(float time, Action method)
    {
        yield return new WaitForSecondsRealtime(time);
        method?.Invoke();
    }

    private static IEnumerator WaitUntilRoutine(Func<bool> predicate, Action method)
    {
        yield return new WaitUntil(predicate);
        method?.Invoke();
    } 
    
    private static IEnumerator WaitWhileRoutine(Func<bool> predicate, Action method)
    {
        yield return new WaitWhile(predicate);
        method?.Invoke();
    } 

    private static IEnumerator WaitEndOfFrameRoutine(Action method)
    {
        yield return new WaitForEndOfFrame();
        method?.Invoke();
    }

    public static CoroutineTask StartCoroutine(IEnumerator routine, string id = "", Action onComplete = null)
    {
        if (_coroutineManager == null) return null;
        if (routine == null)
        {
            Debug.LogError("Coroutine is null");
            return null;
        }

        var coroutineTask = new CoroutineTask(routine, id, Remove);

        if (!string.IsNullOrEmpty(id))
        {
            if (_coroutines.TryGetValue(id, out var result))
            {
                result?.Stop();
            }

            _coroutines[id] = coroutineTask;
        }

        _coroutineManager.StartCoroutine(coroutineTask.Start(onComplete));
        return coroutineTask;
    }

    public static CoroutineTask StartFreeCoroutine(IEnumerator routine, Action onComplete = null)
    {
        if (_freeCoroutineManager == null) return null;
        if (routine == null)
        {
            Debug.LogError("Coroutine is null");
            return null;
        }

        var coroutineTask = new CoroutineTask(routine, "null", null);

        _freeCoroutineManager.StartCoroutine(coroutineTask.Start(onComplete));
        return coroutineTask;
    }

    public static void StopAllCoroutines()
    {
        if (_coroutineManager == null) return;

        StopQueuedGroups();
        StopWhenAllGroups();
        _coroutines.Clear();
        _coroutineManager.StopAllCoroutines();

        Debug.LogWarning("All coroutines are stopped");
    }

    private static void Remove(string id)
    {
        if (!string.IsNullOrEmpty(id))
            _coroutines.Remove(id);
    }

    public static void WhenAll(List<IEnumerator> routines, string group, Action<string> onComplete)
    {
        if (_packedCoroutines.ContainsKey(group)) return;

        _packedCoroutines[group] = routines;

        _coroutines[group] = StartCoroutine(ProcessPackedRoutines(_packedCoroutines[group], group, onComplete));
    }

    public static void StopWhenAllGroup(string group)
    {
        if (!_packedCoroutines.TryGetValue(group, out var queue)) return;

        if (_coroutines.TryGetValue(group, out var task))
            task?.Stop();

        for (var i = 0; i < queue.Count; i++)
        {
            if (_coroutines.TryGetValue(queue[i].ToString(), out task))
                task?.Stop();
        }

        queue.Clear();
        Debug.LogWarning("Packed coroutines are stopped");
    }

    public static void StopWhenAllGroups()
    {
        foreach (var coroutine in _packedCoroutines)
        {
            StopWhenAllGroup(coroutine.Key);
        }

        _packedCoroutines?.Clear();
        Debug.LogWarning("All packed coroutines are stopped");
    }

    public static void AddCoroutineToQueue(IEnumerator routine, string group)
    {
        if (string.IsNullOrEmpty(group)) return;

        if (_queuedCoroutines.TryGetValue(group, out var result))
            result.Enqueue(routine);
        else
        {
            _queuedCoroutines[group] = new Queue<IEnumerator>();
            _queuedCoroutines[group].Enqueue(routine);
            _coroutineManager.StartCoroutine(ProcessQueuedRoutines(group));
        }
    }

    public static void StopQueuedGroup(string group)
    {
        if (!_queuedCoroutines.TryGetValue(group, out var queue)) return;

        if (_coroutines.TryGetValue(group, out var task))
            task?.Stop();

        queue.Clear();
        Debug.LogWarning("Queued coroutines are stopped");
    }

    public static void StopQueuedGroups()
    {
        foreach (var coroutine in _queuedCoroutines)
        {
            if (_coroutines.TryGetValue(coroutine.Key, out var task))
                task?.Stop();
        }

        _queuedCoroutines?.Clear();
        Debug.LogWarning("All queued coroutines are stopped");
    }

    private static IEnumerator ProcessQueuedRoutines(string key)
    {
        while (_queuedCoroutines[key].Count > 0)
        {
            if (_coroutineManager == null) yield break;

            _coroutines[key] = new CoroutineTask(_queuedCoroutines[key].Dequeue(), "", Remove);

            yield return _coroutineManager.StartCoroutine(_coroutines[key].Start(null));
        }

        yield return null;
    }

    private static IEnumerator ProcessPackedRoutines(List<IEnumerator> routines, string group, Action<string> onComplete)
    {
        var coroutineTasks = new List<CoroutineTask>();

        for (var i = 0; i < routines.Count; i++)
        {
            if (_coroutineManager == null) yield break;

            _coroutines[routines[i].ToString()] = new CoroutineTask(routines[i], "", Remove);
            coroutineTasks.Add(_coroutines[routines[i].ToString()]);
            _coroutineManager.StartCoroutine(_coroutines[routines[i].ToString()].Start(null));
        }

        yield return new WaitUntil(() => coroutineTasks.All(c => c.State == CoroutineState.Finished));

        onComplete?.Invoke(group);
        yield return null;
    }
}