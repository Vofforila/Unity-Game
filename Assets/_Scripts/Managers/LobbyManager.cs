using Data;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UI;

namespace TryhardParty
{
    public class LobbyManager : NetworkBehaviour
    {
        [Header("Scriptable")]
        [SerializeField] private LocalData localdata;
        [SerializeField] private Firestore firestore;

        private TMP_InputField chatInput;
        private TMP_InputField chatPanelScrollContent;

        [HideInInspector] internal UIManager uiManager;

        public override void Spawned()
        {
            chatInput = GameObject.Find("ChatInput").GetComponent<TMP_InputField>();
            chatPanelScrollContent = GameObject.Find("ChatPanelScrollContent").GetComponent<TMP_InputField>();
            uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();

            if (Object.HasInputAuthority)
            {
                if (Runner.IsClient == false)
                {
                    firestore.CreateLobbyData();
                }
                else
                {
                    firestore.UpdateLobbyData(localdata.inviteName);
                }
                firestore.RaiseLobbyDataListener(Runner.SessionInfo.Name);
            }
        }

        public void Update()
        {
            if (Object.HasInputAuthority)
            {
                // Send ChatMsg
                if (localdata.returnKeyPressed == true && chatInput.text != "")
                {
                    RPC_SendMessage(chatInput.text, firestore.accountFirebase.User);
                    chatInput.text = "";
                    localdata.returnKeyPressed = false;
                }
            }
        }

        public void UpdatePlayerBannerEvent()
        {
            Debug.Log("Callback");

            if (Object.HasInputAuthority)
            {
                // Enable Player Banner
                int playerCount = firestore.lobbydata.HostPlayerList.Count + 1;

                uiManager.EnableHomePanel(false);
                uiManager.EnableLobbyPanel(true);

                if (playerCount > 1)
                {
                    uiManager.EnableChatBoxPanel(true);
                }
                else
                {
                    uiManager.EnableChatBoxPanel(false);
                }

                if (playerCount == 1)
                {
                    uiManager.EnablePlayerBanner1(true);
                }
                if (playerCount == 2)
                {
                    uiManager.EnablePlayerBanner2(true);
                }
                if (playerCount == 3)
                {
                    uiManager.EnablePlayerBanner3(true);
                }
                if (playerCount == 4)
                {
                    uiManager.EnablePlayerBanner4(true);
                }

                // Update Player Banner Content
                TMP_Text[] textPlayerBannerList = GameObject.Find("BannerLayout").GetComponentsInChildren<TMP_Text>();

                for (int i = 0; i < firestore.lobbydata.HostPlayerList.Count; i++)
                {
                    string username = (firestore.lobbydata.HostPlayerList[i]).ToString();
                    textPlayerBannerList[i].text = username;
                }
            }
        }

        // Send Rpc To Server
        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
        public void RPC_SendMessage(string _message, string _username)
        {
            RPC_RelayMessage(_message, _username);
        }

        // Send Rpc from Server to All Clients
        [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
        private void RPC_RelayMessage(string _message, string _username)
        {
            chatPanelScrollContent.text += _username + " : " + _message + "\n";
        }
    }
}