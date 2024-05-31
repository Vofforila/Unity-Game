using Data;
using Database;
using Server;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TryhardParty
{
    public class LobbyInvitePrefab : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button acceptInvite;
        [SerializeField] private Button declineInvite;

        [Header("Object")]
        [SerializeField] private TMP_Text username;

        [Header("Scriptable")]
        [SerializeField] private LocalData localData;
        [SerializeField] private Firestore firestore;

        [Header("Event")]
        public UnityEvent InviteEvent;

        private void Awake()
        {
            acceptInvite.onClick.AddListener(AcceptInvite);
            declineInvite.onClick.AddListener(DeclineInvite);
        }

        public void AcceptInvite()
        {
            firestore.RemoveInvite(gameObject.name);
            FusionManager.Instance.InviteResponseEvent(username.text);
        }

        public void DeclineInvite()
        {
            firestore.RemoveInvite(gameObject.name);
        }
    }
}