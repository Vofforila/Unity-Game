using Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "ShowStatisticPanelEvent", menuName = "Event/ShowStatisticPanelEvent")]
public class ShowStatisticPanelEvent : ScriptableObject
{
    private List<ShowStatisticPanelEventListener> listeners = new();

    public void Raise()
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
        {
            listeners[i].OnEventRaised();
        }
    }

    public void RegisterListener(ShowStatisticPanelEventListener listener)
    {
        listeners.Add(listener);
    }

    public void UnregisterListener(ShowStatisticPanelEventListener listener)
    {
        listeners.Remove(listener);
    }
}