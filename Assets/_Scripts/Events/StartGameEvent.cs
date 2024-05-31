using System.Collections.Generic;
using UnityEngine;

namespace Event
{
    [CreateAssetMenu(fileName = "StartGameEvent", menuName = "Event/StartGameEvent")]
    public class StartGameEvent : ScriptableObject
    {
        private List<StartGameEventListener> listeners = new();

        public void Raise()
        {
            for (int i = listeners.Count - 1; i >= 0; i--)
            {
                listeners[i].OnEventRaised();
            }
        }

        public void RegisterListener(StartGameEventListener listener)
        {
            listeners.Add(listener);
        }

        public void UnregisterListener(StartGameEventListener listener)
        {
            listeners.Remove(listener);
        }
    }
}