using System.Collections.Generic;
using UnityEngine;

namespace Event
{
    [CreateAssetMenu(fileName = "PlayLevel3Event", menuName = "Event/PlayLevel3Event")]
    public class PlayLevel3Event : ScriptableObject
    {
        private List<PlayLevel3EventListener> listeners = new();

        public void Raise()
        {
            for (int i = listeners.Count - 1; i >= 0; i--)
            {
                listeners[i].OnEventRaised();
            }
        }

        public void RegisterListener(PlayLevel3EventListener listener)
        {
            listeners.Add(listener);
        }

        public void UnregisterListener(PlayLevel3EventListener listener)
        {
            listeners.Remove(listener);
        }
    }
}