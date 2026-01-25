using System;
using System.Collections.Concurrent;
using System.Threading;
using UnityEngine;
public class MainThread : MonoBehaviour
{
    static readonly ConcurrentQueue<Action> q = new();
    static int mainId;
    void Awake() { mainId = Thread.CurrentThread.ManagedThreadId; DontDestroyOnLoad(gameObject); }
    public static void Post(Action a) { if (a != null) q.Enqueue(a); }
    public static bool IsMain => Thread.CurrentThread.ManagedThreadId == mainId;
    void Update() { while (q.TryDequeue(out var a)) try { a(); } catch (Exception e) { Debug.LogException(e); } }
}
