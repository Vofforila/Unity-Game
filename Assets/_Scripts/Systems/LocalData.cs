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

        public string gameChat;

        [Header("PlayerData")]
        public int playersAlive;

        [Header("Chatbox")]
        public bool returnKeyPressed = false;

        [Header("Current Level")]
        public int currentLvl;
        public List<string> lobbyPlayerList = new();

        [Header("Level 1 Data")]
        public bool levelFinished;
        public bool playerTurn;

        [Header("Match Statistics")]
        public bool showMatchStatistics = false;
    }
}