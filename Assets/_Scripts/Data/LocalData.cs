using Firebase.Firestore;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Data
{
    [CreateAssetMenu(fileName = "LocalData", menuName = "Data/LocalData")]
    public class LocalData : ScriptableObject
    {
        [Header("InvitetoLobby")]
        public PointerEventData pointerData;
        public string FriendClicked;
        public bool inviteResponse;
        public string inviteName;

        public Dictionary<PlayerRef, string> playerList;

        public string gameChat;

        [Header("PlayerData")]
        public int playersAlive;

        [Header("Chatbox")]
        public bool returnKeyPressed = false;

        [Header("Current Level")]
        public int currentLvl = 0;
        public List<string> lobbyPlayerList = new();

        [Header("Level 1 Data")]
        public bool levelFinished;
        public bool playerTurn;

        [Header("Match Statistics")]
        public bool showMatchStatistics;
    }
}