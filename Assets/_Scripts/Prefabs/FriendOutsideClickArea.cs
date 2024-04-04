using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class FriendOutsideClickArea : MonoBehaviour, IPointerClickHandler
    {
        [Header("Scriptable")]
        [SerializeField] private LocalData localData;

        [Header("Objects")]
        [SerializeField] private GameObject rightClickPanelPrefab;

        private GameObject instanciatedRightClickPanelPrefab;

        public void Awake()
        {
            instanciatedRightClickPanelPrefab = Instantiate(rightClickPanelPrefab, gameObject.transform);
            instanciatedRightClickPanelPrefab.SetActive(false);
        }

        // Listens the Pointer
        public void OnPointerClick(PointerEventData eventData)
        {
            if (instanciatedRightClickPanelPrefab.activeSelf == true)
            {
                instanciatedRightClickPanelPrefab.SetActive(false);
            }
        }

        public void HandleRightClickPanel()
        {
            Debug.Log("Callback");
            // Detects if the button pressed is Rick-Click
            if (localData.pointerData.button == PointerEventData.InputButton.Right)
            {
                instanciatedRightClickPanelPrefab.transform.position = localData.pointerData.position;
                instanciatedRightClickPanelPrefab.SetActive(true);
            }
            if (localData.pointerData.button == PointerEventData.InputButton.Left)
            {
                instanciatedRightClickPanelPrefab.SetActive(false);
            }
        }
    }
}