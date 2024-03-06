using System.Collections.Generic;
using UnityEngine;

namespace Event
{
    [CreateAssetMenu(fileName = "HandleRightClickPanelEvent", menuName = "Event/HandleRightClickPanelEvent")]
    public class HandleRightClickPanelEvent : ScriptableObject
    {
        private List<HandleRightClickPanelEventListener> listeners = new();

        public void Raise()
        {
            for (int i = listeners.Count - 1; i >= 0; i--)
            {
                listeners[i].OnEventRaised();
            }
        }

        public void RegisterListener(HandleRightClickPanelEventListener listener)
        {
            listeners.Add(listener);
        }

        public void UnregisterListener(HandleRightClickPanelEventListener listener)
        {
            listeners.Remove(listener);
        }
    }
}