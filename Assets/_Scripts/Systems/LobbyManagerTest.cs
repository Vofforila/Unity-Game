using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Data;

namespace TryhardParty
{
    public class LobbyManagerTest : NetworkBehaviour
    {
        [SerializeField] private Firestore firestore;
        [SerializeField] private LocalData localData;

        [HideInInspector] internal LobbyUIManager lobbyUIManager;

        [Networked, HideInInspector] public NetworkString<_16> UserName { get; private set; }

        private ChangeDetector changeDetector;

        public override void Spawned()
        {
            lobbyUIManager = GameObject.Find("LobbyUIManager").GetComponent<LobbyUIManager>();

            // Client
            if (Object.HasInputAuthority)
            {
                RPC_SendUsername(firestore.accountFirebase.User);
            }
            // Host
            if (Object.HasStateAuthority)
            {
            }

            // Create UI
            lobbyUIManager.CreateUI(Object.InputAuthority);

            // Update UI
            lobbyUIManager.UpdateUserName(Object.InputAuthority, UserName.ToString());

            // Activate Listener
            changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
        }

        // Update UI for Other Users
        public override void Render()
        {
            foreach (var change in changeDetector.DetectChanges(this, out var previousBuffer, out var currentBuffer))
            {
                switch (change)
                {
                    case nameof(UserName):
                        lobbyUIManager.UpdateUserName(Object.InputAuthority, UserName.ToString());
                        break;
                }
            }
        }

        // RPC used to send player information to the Host
        [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
        private void RPC_SendUsername(string _username)
        {
            UserName = _username;
        }
    }
}