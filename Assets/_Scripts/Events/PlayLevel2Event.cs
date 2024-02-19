using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TryhardParty
{
    [CreateAssetMenu(fileName = "PlayLevel2Event", menuName = "Event/PlayLevel2Event")]
    public class PlayLevel2Event : ScriptableObject
    {
        private List<PlayLevel2EventListener> listeners = new();

        public void Raise()
        {
            for (int i = listeners.Count - 1; i >= 0; i--)
            {
                listeners[i].OnEventRaised();
            }
        }

        public void RegisterListener(PlayLevel2EventListener listener)
        {
            listeners.Add(listener);
        }

        public void UnregisterListener(PlayLevel2EventListener listener)
        {
            listeners.Remove(listener);
        }
    }
}