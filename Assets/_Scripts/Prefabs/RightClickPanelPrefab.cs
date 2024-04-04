using UnityEngine;
using UnityEngine.UI;
using Data;
using Database;

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