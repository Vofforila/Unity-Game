using System.Collections.Generic;
using UnityEngine;

namespace Event
{
    [CreateAssetMenu(fileName = "PlayerJoinedLobbyEvent", menuName = "Event/PlayerJoinedLobbyEvent")]
    public class PlayerJoinedLobbyEvent : ScriptableObject
    {
        private List<PlayerJoinedLobbyEventListener> listeners = new();

        public void Raise()
        {
            for (int i = listeners.Count - 1; i >= 0; i--)
            {
                listeners[i].OnEventRaised();
            }
        }

        public void RegisterListener(PlayerJoinedLobbyEventListener listener)
        {
            listeners.Add(listener);
        }

        public void UnregisterListener(PlayerJoinedLobbyEventListener listener)
        {
            listeners.Remove(listener);
        }
    }
}