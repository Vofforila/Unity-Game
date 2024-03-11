using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;
using Database;

namespace Host
{
    public class ChatManager : NetworkBehaviour
    {
        [Header("Scriptable")]
        [SerializeField] private Firestore firestore;

        private TMP_InputField chatInput;
        private TMP_InputField chatPanelScrollContent;

        private void Awake()
        {
            chatInput = GameObject.Find("ChatInput").GetComponent<TMP_InputField>();
            chatPanelScrollContent = GameObject.Find("ChatPanelScrollContent").GetComponent<TMP_InputField>();
        }

        public void SendMessage()
        {
            if (chatInput.text != "")
            {
                Debug.Log(firestore.accountFirebase.User);
                RPC_RelayMessage(chatInput.text, firestore.accountFirebase.User);
                chatInput.text = "";
            }
        }

        [Rpc(sources: RpcSources.Proxies, targets: RpcTargets.StateAuthority)]
        private void RPC_RelayMessage(string _message, string _username)
        {
            RPC_Message(_message, _username);
        }

        [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.Proxies)]
        private void RPC_Message(string _message, string _username)
        {
            chatPanelScrollContent.text += _username + " : " + _message + "\n";
        }
    }
}