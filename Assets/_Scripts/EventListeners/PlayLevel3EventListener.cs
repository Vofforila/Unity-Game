using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TryhardParty
{
    public class PlayLevel3EventListener : MonoBehaviour
    {
        public PlayLevel3Event Event;
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