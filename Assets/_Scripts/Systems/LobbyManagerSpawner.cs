using System.Collections;
using UnityEngine;
using Fusion;
using UI;
using System.Collections.Generic;

namespace Host
{
    public class LobbyManagerSpawner : NetworkBehaviour, IPlayerJoined
    {
        [Header("Prefab")]
        [SerializeField] private NetworkPrefabRef lobbyManagerPrefab;

        public void PlayerJoined(PlayerRef player)
        {
            StartCoroutine(WaitForUpdate(player));
        }

        public void Dissconnect()
        {
            Runner.Disconnect(Object.InputAuthority);
        }

        // Wait to Process User
        public IEnumerator WaitForUpdate(PlayerRef _player)
        {
            yield return new WaitForFixedUpdate();
            Runner.Spawn(lobbyManagerPrefab, Vector3.zero, Quaternion.identity, _player);
        }

        public void ShowStatisticPanelEvent()
        {
            Debug.Log("Callback");
            RPC_ShowStatisticEvent();
        }

        [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
        private void RPC_ShowStatisticEvent()
        {
            Debug.Log("Callback rpc");
            UIManager.Instance.ShowStatisticPanelEvent();
        }
    }
}