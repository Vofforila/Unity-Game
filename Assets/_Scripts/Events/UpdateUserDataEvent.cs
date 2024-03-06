using System.Collections.Generic;
using UnityEngine;

namespace Event
{
    [CreateAssetMenu(fileName = "UpdateUserDataEvent", menuName = "Event/UpdateUserDataEvent")]
    public class UpdateUserDataEvent : ScriptableObject
    {
        private List<UpdateUserDataListener> listeners = new();

        public void Raise()
        {
            for (int i = listeners.Count - 1; i >= 0; i--)
            {
                listeners[i].OnEventRaised();
            }
        }

        public void RegisterListener(UpdateUserDataListener listener)
        {
            listeners.Add(listener);
        }

        public void UnregisterListener(UpdateUserDataListener listener)
        {
            listeners.Remove(listener);
        }
    }
}