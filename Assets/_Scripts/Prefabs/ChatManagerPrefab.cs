using Data;
using Database;
using Fusion;
using Server;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Host
{
    public class ChatManagerPrefab : MonoBehaviour
    {
        internal FusionManager fusionManager;

        [SerializeField] private Firestore firestore;
        [SerializeField] private LocalData localData;

        private TMP_InputField chatInput;
        private TMP_InputField chatPanelScrollContent;

        public void OnEnable()
        {
            fusionManager = GameObject.Find("FusionManager").GetComponent<FusionManager>();
            chatInput = GameObject.Find("ChatInput").GetComponent<TMP_InputField>();
            chatPanelScrollContent = GameObject.Find("ChatPanelScrollContent").GetComponent<TMP_InputField>();
            chatPanelScrollContent.text = localData.gameChat;
            localData.gameChat = "";
        }

        public void OnDisable()
        {
            localData.gameChat = chatPanelScrollContent.text;
        }

        public void SendMessageToServer()
        {
            if (Input.GetKeyDown(KeyCode.Return) && chatInput.text != "")
            {
                string username = firestore.accountFirebase.User;
                string message = chatInput.text;
                string finalmessage = username + " : " + message + "\n";

                firestore.UpdateChat(finalmessage, fusionManager.runner.SessionInfo.Name);
                chatInput.text = "";
            }
        }

        public void UpdateLobbyDataEvent()
        {
            Debug.Log("Callback");
            chatPanelScrollContent.text = firestore.lobbydata.RoomChat;
        }
    }
}