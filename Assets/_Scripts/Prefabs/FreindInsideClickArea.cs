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
        public TMP_Text friendUsername;
        public UnityEvent HandleRightClickPanelEvent;
        public LocalData localData;

        // Listens the Pointer
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right || eventData.button == PointerEventData.InputButton.Left)
            {
                Debug.Log("DisableRightClickPanelEvent");
                localData.FriendClicked = friendUsername.text;
                localData.pointerData = eventData;
                HandleRightClickPanelEvent.Invoke();
            }
        }
    }
}