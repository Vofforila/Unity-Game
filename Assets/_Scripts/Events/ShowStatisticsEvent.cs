using System.Collections.Generic;
using UnityEngine;

namespace Event
{
    [CreateAssetMenu(fileName = "ShowStatisticsEvent", menuName = "Event/ShowStatisticsEvent")]
    public class ShowStatisticsEvent : ScriptableObject
    {
        private List<ShowStatisticsEventListener> listeners = new();

        public void Raise()
        {
            for (int i = listeners.Count - 1; i >= 0; i--)
            {
                listeners[i].OnEventRaised();
            }
        }

        public void RegisterListener(ShowStatisticsEventListener listener)
        {
            listeners.Add(listener);
        }

        public void UnregisterListener(ShowStatisticsEventListener listener)
        {
            listeners.Remove(listener);
        }
    }
}