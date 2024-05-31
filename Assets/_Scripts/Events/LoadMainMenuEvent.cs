using System.Collections.Generic;
using UnityEngine;

namespace Event
{
    [CreateAssetMenu(fileName = "LoadMainMenuEvent", menuName = "Event/LoadMainMenuEvent")]
    public class LoadMainMenuEvent : ScriptableObject
    {
        private List<LoadMainMenuEventListener> listeners = new();

        public void Raise()
        {
            for (int i = listeners.Count - 1; i >= 0; i--)
            {
                listeners[i].OnEventRaised();
            }
        }

        public void RegisterListener(LoadMainMenuEventListener listener)
        {
            listeners.Add(listener);
        }

        public void UnregisterListener(LoadMainMenuEventListener listener)
        {
            listeners.Remove(listener);
        }
    }
}