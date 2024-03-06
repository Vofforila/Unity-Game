using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using Data;

namespace TryhardParty
{
    public class LobbyInvitePrefab : MonoBehaviour
    {
        [Header("Buttons")]
        public Button acceptInvite;
        public Button declineInvite;

        [Header("Invite Data")]
        public TMP_Text inviteName;

        [Header("Scriptable")]
        public LocalData localData;

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
            localData.inviteResponse = true;
            localData.inviteName = inviteName.text;
            InviteEvent.Invoke();
        }

        public void DeclineInvite()
        {
            Debug.Log("Invite Response Event");
            localData.inviteResponse = false;
            localData.inviteName = inviteName.text;
            InviteEvent.Invoke();
        }
    }
}