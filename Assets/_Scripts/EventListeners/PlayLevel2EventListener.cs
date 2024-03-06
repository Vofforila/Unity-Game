using UnityEngine;
using UnityEngine.Events;

namespace Event
{
    public class PlayLevel2EventListener : MonoBehaviour
    {
        public PlayLevel2Event Event;
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
}