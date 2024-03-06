using System.Collections.Generic;
using UnityEngine;

namespace Event
{
    [CreateAssetMenu(fileName = "LoadLevel4Event", menuName = "Event/LoadLevel4Event")]
    public class LoadLevel4Event : ScriptableObject
    {
        private List<LoadLevel4EventListener> listeners = new();

        public void Raise()
        {
            for (int i = listeners.Count - 1; i >= 0; i--)
            {
                listeners[i].OnEventRaised();
            }
        }

        public void RegisterListener(LoadLevel4EventListener listener)
        {
            listeners.Add(listener);
        }

        public void UnregisterListener(LoadLevel4EventListener listener)
        {
            listeners.Remove(listener);
        }
    }
}