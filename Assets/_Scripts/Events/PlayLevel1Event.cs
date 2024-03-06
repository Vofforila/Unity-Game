using System.Collections.Generic;
using UnityEngine;

namespace Event
{
    [CreateAssetMenu(fileName = "PlayLevel1Event", menuName = "Event/PlayLevel1Event")]
    public class PlayLevel1Event : ScriptableObject
    {
        private List<PlayLevel1EventListener> listeners = new();

        public void Raise()
        {
            for (int i = listeners.Count - 1; i >= 0; i--)
            {
                listeners[i].OnEventRaised();
            }
        }

        public void RegisterListener(PlayLevel1EventListener listener)
        {
            listeners.Add(listener);
        }

        public void UnregisterListener(PlayLevel1EventListener listener)
        {
            listeners.Remove(listener);
        }
    }
}