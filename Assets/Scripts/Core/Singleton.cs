using System;
using System.Diagnostics;
using System.Reflection;

public abstract class Singleton<T>
    where T : new()
{
    static T _instance;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public static T instance => _instance ?? (_instance = new T());

    public static void ResetSingleton()
    {
        if (_instance is IDisposable disposable)
            disposable?.Dispose();
        _instance = default;
    }
    
}