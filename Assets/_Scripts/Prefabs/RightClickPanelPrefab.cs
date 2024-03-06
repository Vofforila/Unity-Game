using UnityEngine;
using UnityEngine.UI;
using Data;
using Database;

namespace UI
{
    public class RightClickPanelPrefab : MonoBehaviour
    {
        [Header("Buttons")]
        public Button InviteToLobbyButton;
        public Button ViewProfileButton;

        [Header("Scriptables")]
        public Firestore firestore;
        public LocalData localData;

        private void Awake()
        {
            InviteToLobbyButton.onClick.AddListener(InviteToLobby);
            InviteToLobbyButton.onClick.AddListener(ViewProfile);
        }

        public void InviteToLobby()
        {
            firestore.InviteToLobby(localData.FriendClicked);
        }

        public void ViewProfile()
        {
        }
    }
}