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

        // Wait to Process User
        public IEnumerator WaitForUpdate(PlayerRef _player)
        {
            yield return new WaitForFixedUpdate();
            Runner.Spawn(lobbyManagerPrefab, Vector3.zero, Quaternion.identity, _player);
        }
    }
}