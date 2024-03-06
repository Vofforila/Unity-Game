using System.Collections.Generic;
using UnityEngine;

namespace Event
{
    [CreateAssetMenu(fileName = "UpdateLobbyDataEvent", menuName = "Event/UpdateLobbyDataEvent")]
    public class UpdateLobbyDataEvent : ScriptableObject
    {
        private List<UpdateLobbyDataEventListener> listeners = new();

        public void Raise()
        {
            for (int i = listeners.Count - 1; i >= 0; i--)
            {
                listeners[i].OnEventRaised();
            }
        }

        public void RegisterListener(UpdateLobbyDataEventListener listener)
        {
            listeners.Add(listener);
        }

        public void UnregisterListener(UpdateLobbyDataEventListener listener)
        {
            listeners.Remove(listener);
        }
    }
}