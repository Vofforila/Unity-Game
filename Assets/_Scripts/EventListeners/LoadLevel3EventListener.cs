using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TryhardParty
{
    public class LoadLevel3EventListener : MonoBehaviour
    {
        public LoadLevel3Event Event;
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