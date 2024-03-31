using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class FriendOutsideClickArea : MonoBehaviour, IPointerClickHandler
    {
        public GameObject rightClickPanelPrefab;
        public Transform MainMenuCanvasTransform;

        public LocalData localData;

        [SerializeField]
        public GameObject instanciatedRightClickPanelPrefab;

        public void Awake()
        {
            instanciatedRightClickPanelPrefab = Instantiate(rightClickPanelPrefab, MainMenuCanvasTransform);
            instanciatedRightClickPanelPrefab.transform.position = new Vector3(3000, 3000, 0);
        }

        // Listens the Pointer
        public void OnPointerClick(PointerEventData eventData)
        {
            if (instanciatedRightClickPanelPrefab.activeSelf == true)
                instanciatedRightClickPanelPrefab.SetActive(false);
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
            else if (localData.pointerData.button == PointerEventData.InputButton.Left)
            {
                instanciatedRightClickPanelPrefab.SetActive(false);
            }
        }
    }
}