using UnityEngine;
using UnityEngine.Events;

namespace Event
{
    public class LoadLevel4EventListener : MonoBehaviour
    {
        public LoadLevel4Event Event;
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