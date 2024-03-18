using Data;
using Database;
using Fusion;
using Server;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UI
{
    public class ChatManagerPrefab : NetworkBehaviour
    {
        internal FusionManager fusionManager;

        [SerializeField] private Firestore firestore;
        [SerializeField] private LocalData localData;

        private TMP_InputField chatInput;
        private TMP_InputField chatPanelScrollContent;
        private string finalmessage;

        private void Awake()
        {
            chatInput = GameObject.Find("ChatInput").GetComponent<TMP_InputField>();
            chatPanelScrollContent = GameObject.Find("ChatPanelScrollContent").GetComponent<TMP_InputField>();
        }

        public void SendMessageToServer()
        {
            if (Input.GetKeyDown(KeyCode.Return) && chatInput.text != "")
            {
                string username = firestore.accountFirebase.User;
                string message = chatInput.text;
                finalmessage = username + " : " + message + "\n";
                chatInput.text = "";
                RPC_RelaySendMessage(finalmessage);
            }
        }

        [Rpc(sources: RpcSources.All, targets: RpcTargets.StateAuthority)]
        private void RPC_RelaySendMessage(string _finalmessage, RpcInfo info = default)
        {
            RPC_SendMessage(_finalmessage);
        }

        [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
        private void RPC_SendMessage(string _finalmessage)
        {
            chatPanelScrollContent.text += _finalmessage;
        }
    }
}