using Data;
using Database;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class RightClickPanelPrefab : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button InviteToLobbyButton;
        [SerializeField] private Button ViewProfileButton;

        [Header("Scriptables")]
        [SerializeField] private Firestore firestore;
        [SerializeField] private LocalData localData;

        private void Awake()
        {
            InviteToLobbyButton.onClick.AddListener(InviteToLobby);
            ViewProfileButton.onClick.AddListener(ViewProfile);
        }

        private void OnEnable()
        {
            GameObject lobbyPanel = GameObject.Find("LobbyPanel");
            if (lobbyPanel != null)
            {
                InviteToLobbyButton.interactable = true;
            }
            else
            {
                InviteToLobbyButton.interactable = false;
            }
        }

        public void InviteToLobby()
        {
            firestore.InviteToLobby(localData.FriendClicked);
        }

        public void ViewProfile()
        {
            UIManager.Instance.UpdateFriendProfile(localData.FriendClicked);
        }
    }
}