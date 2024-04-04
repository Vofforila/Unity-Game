using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Data;
using Database;
using TMPro;

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
            Debug.Log("Invite Response Event");
            localData.inviteName = username.text;
            InviteEvent.Invoke();
        }

        public void DeclineInvite()
        {
            firestore.RemoveInvite(gameObject.name);
        }
    }
}