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

//public class UIHelper
//{
//    public static void TriggerOnDeselect(GameObject target)
//    {
//        if (target != null)
//        {
//            ExecuteEvents.Execute<IDeselectHandler>(
//                target,
//                new BaseEventData(EventSystem.current),
//                ExecuteEvents.deselectHandler
//            );
//        }
//    }
//}