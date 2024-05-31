using Data;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class FreindInsideClickArea : MonoBehaviour, IPointerClickHandler
    {
        [Header("Scriptable")]
        [SerializeField] private LocalData localData;

        [Header("Event")]
        public UnityEvent HandleRightClickPanelEvent;

        // Listens the Pointer
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right || eventData.button == PointerEventData.InputButton.Left)
            {
                Debug.Log("FriendInsideClickEvent");
                localData.FriendClicked = gameObject.name;
                localData.pointerData = eventData;
                HandleRightClickPanelEvent.Invoke();
            }
        }
    }
}