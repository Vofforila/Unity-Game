using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TryhardParty
{
    [CreateAssetMenu(fileName = "DestroyPlayerEvent", menuName = "Event/DestroyPlayerEvent")]
    public class DestroyPlayerEvent : ScriptableObject
    {
        private List<DestroyPlayerEventListener> listeners = new();

        public void Raise()
        {
            for (int i = listeners.Count - 1; i >= 0; i--)
            {
                listeners[i].OnEventRaised();
            }
        }

        public void RegisterListener(DestroyPlayerEventListener listener)
        {
            listeners.Add(listener);
        }

        public void UnregisterListener(DestroyPlayerEventListener listener)
        {
            listeners.Remove(listener);
        }
    }
}