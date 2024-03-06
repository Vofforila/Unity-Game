using System.Collections.Generic;
using UnityEngine;

namespace Event
{
    [CreateAssetMenu(fileName = "PlayerTurnEvent", menuName = "Event/PlayerTurnEvent")]
    public class PlayerTurnEvent : ScriptableObject
    {
        private List<PlayerTurnEventListener> listeners = new();

        public void Raise()
        {
            for (int i = listeners.Count - 1; i >= 0; i--)
            {
                listeners[i].OnEventRaised();
            }
        }

        public void RegisterListener(PlayerTurnEventListener listener)
        {
            listeners.Add(listener);
        }

        public void UnregisterListener(PlayerTurnEventListener listener)
        {
            listeners.Remove(listener);
        }
    }
}