using UnityEngine;

public static class DebugManager
{
    // This flag allows you to toggle debug logs in development builds.
    public static bool IsDebugEnabled { get; set; } = false;

    public static void Log(object message)
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (IsDebugEnabled)
        {
            Debug.Log(message);
        }
#endif
    }

    public static void LogWarning(object message)
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (IsDebugEnabled)
        {
            Debug.LogWarning(message);
        }
#endif
    }

    public static void LogError(object message)
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (IsDebugEnabled)
        {
            Debug.LogError(message);
        }
#endif
    }
}