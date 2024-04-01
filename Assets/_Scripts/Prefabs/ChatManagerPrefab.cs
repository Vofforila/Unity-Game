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
        [Header("Scriptable")]
        [SerializeField] private Firestore firestore;

        [SerializeField] private TMP_InputField chatInput;
        [SerializeField] private TMP_InputField chatPanelScrollContent;
        private string finalmessage;

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