using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TryhardParty
{
    [CreateAssetMenu(fileName = "LoadLevel2Event", menuName = "Event/LoadLevel2Event")]
    public class LoadLevel2Event : ScriptableObject
    {
        private List<LoadLevel2EventListener> listeners = new();

        public void Raise()
        {
            for (int i = listeners.Count - 1; i >= 0; i--)
            {
                listeners[i].OnEventRaised();
            }
        }

        public void RegisterListener(LoadLevel2EventListener listener)
        {
            listeners.Add(listener);
        }

        public void UnregisterListener(LoadLevel2EventListener listener)
        {
            listeners.Remove(listener);
        }
    }
}