using Database;
using Fusion;
using Server;
using TMPro;
using UnityEngine;

namespace UI
{
    public class ChatManagerPrefab : SimulationBehaviour
    {
        [Header("Scriptable")]
        [SerializeField] private Firestore firestore;

        [SerializeField] private TMP_InputField chatInput;
        [SerializeField] private TMP_InputField chatPanelScrollContent;
        private string finalmessage;

        public static ChatManagerPrefab Instance;

        private void Awake()
        {
            Instance = this;
        }

        public void SendMessageToServer()
        {
            if (Input.GetKeyDown(KeyCode.Return) && chatInput.text != "")
            {
                string username = firestore.accountFirebase.User;
                string message = chatInput.text;
                finalmessage = username + " : " + message + "\n";
                chatInput.text = "";
                RPC_SendMessage(FusionManager.Instance.runner, finalmessage);
            }
        }

        [Rpc]
        public static void RPC_SendMessage(NetworkRunner runner, string _finalmessage)
        {
            Instance.chatPanelScrollContent.text += _finalmessage;
        }
    }
}