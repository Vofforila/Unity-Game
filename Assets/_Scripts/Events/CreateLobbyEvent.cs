using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CreateLobbyEvent", menuName = "Event/CreateLobbyEvent")]
public class CreateLobbyEvent : ScriptableObject
{
    private List<CreateLobbyEventListner> listeners = new();

    public void Raise()
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
        {
            listeners[i].OnEventRaised();
        }
    }

    public void RegisterListener(CreateLobbyEventListner listener)
    {
        listeners.Add(listener);
    }

    public void UnregisterListener(CreateLobbyEventListner listener)
    {
        listeners.Remove(listener);
    }
}