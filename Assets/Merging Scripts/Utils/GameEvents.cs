using UnityEngine;
using System;
//using UnityEngine.EventSystems;

public static class GameEvents
{
    public static event Action onInteractStop;

    public static void OnInteractStop()
    {
        onInteractStop?.Invoke();
    }
}

