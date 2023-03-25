using System;
using UnityEngine.Networking;
using System.Runtime.CompilerServices;
using UnityEngine;

public struct UnityWebRequestAwaiter : INotifyCompletion
{
    private UnityWebRequestAsyncOperation _asyncOperation;
    private Action _continuation;

    public UnityWebRequestAwaiter(UnityWebRequestAsyncOperation asyncOperation)
    {
        this._asyncOperation = asyncOperation;
        _continuation = null;
    }

    public bool IsCompleted { get { return _asyncOperation.isDone; } }

    public void GetResult() { }

    public void OnCompleted(Action continuation)
    {
        this._continuation = continuation;
        _asyncOperation.completed += OnRequestCompleted;
    }

    private void OnRequestCompleted(AsyncOperation obj)
    {
        _continuation?.Invoke();
    }
}

public static class ExtensionMethods
{
    public static UnityWebRequestAwaiter GetAwaiter(this UnityWebRequestAsyncOperation asyncOp)
    {
        return new UnityWebRequestAwaiter(asyncOp);
    }
}