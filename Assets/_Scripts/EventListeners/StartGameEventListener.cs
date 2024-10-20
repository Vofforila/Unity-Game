using Event;
using UnityEngine;
using UnityEngine.Events;

public class StartGameEventListener : MonoBehaviour
{
    public StartGameEvent Event;
    public UnityEvent Response;

    private void OnEnable()
    {
        Event.RegisterListener(this);
    }

    private void OnDisable()
    {
        Event.UnregisterListener(this);
    }

    public void OnEventRaised()
    {
        Response.Invoke();
    }
}