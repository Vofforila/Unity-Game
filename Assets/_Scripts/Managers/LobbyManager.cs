using UnityEngine;
using Fusion;
using Data;
using Database;
using UI;

namespace Host
{
    public class LobbyManager : NetworkBehaviour, IPlayerLeft
    {
        [SerializeField] private Firestore firestore;
        [SerializeField] private LocalData localData;

        private LobbyUIListener LobbyUIListener;

        [Networked, HideInInspector] private NetworkString<_16> UserName { get; set; }
        [Networked, HideInInspector] private PlayerRef PlayerDc { get; set; }

        private ChangeDetector changeDetector;

        public override void Spawned()
        {
            LobbyUIListener = LobbyUIListener.Instance;

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
            LobbyUIListener.CreateUI(Object.InputAuthority);

            // Update UI
            LobbyUIListener.UpdateUserName(Object.InputAuthority, UserName.ToString());

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
                        LobbyUIListener.UpdateUserName(Object.InputAuthority, UserName.ToString());
                        break;
                    case nameof(PlayerDc):
                        LobbyUIListener.RemovePlayer(PlayerDc);
                        break;
                }
            }
        }

        public void PlayerLeft(PlayerRef player)
        {
            PlayerDc = player;
        }

        // RPC used to send player information to the Host
        [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
        private void RPC_SendUsername(string _username)
        {
            UserName = _username;
        }
    }
}