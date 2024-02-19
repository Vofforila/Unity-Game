using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TryhardParty
{
    [CreateAssetMenu(fileName = "PlayLevel4Event", menuName = "Event/PlayLevel4Event")]
    public class PlayLevel4Event : ScriptableObject
    {
        private List<PlayLevel4EventListener> listeners = new();

        public void Raise()
        {
            for (int i = listeners.Count - 1; i >= 0; i--)
            {
                listeners[i].OnEventRaised();
            }
        }

        public void RegisterListener(PlayLevel4EventListener listener)
        {
            listeners.Add(listener);
        }

        public void UnregisterListener(PlayLevel4EventListener listener)
        {
            listeners.Remove(listener);
        }
    }
}