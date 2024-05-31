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

        [Header("Current Level")]
        public int currentLvl;
        /* public List<string> lobbyPlayerList = new();*/

        /*   [Header("Level 1 Data")]
           public bool levelFinished;
           public bool playerTurn;*/

        /*  [Header("Match Statistics")]
          public bool showMatchStatistics = false;*/
    }
}