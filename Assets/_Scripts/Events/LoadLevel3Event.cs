using System.Collections.Generic;
using UnityEngine;

namespace Event
{
    [CreateAssetMenu(fileName = "LoadLevel3Event", menuName = "Event/LoadLevel3Event")]
    public class LoadLevel3Event : ScriptableObject
    {
        private List<LoadLevel3EventListener> listeners = new();

        public void Raise()
        {
            for (int i = listeners.Count - 1; i >= 0; i--)
            {
                listeners[i].OnEventRaised();
            }
        }

        public void RegisterListener(LoadLevel3EventListener listener)
        {
            listeners.Add(listener);
        }

        public void UnregisterListener(LoadLevel3EventListener listener)
        {
            listeners.Remove(listener);
        }
    }
}