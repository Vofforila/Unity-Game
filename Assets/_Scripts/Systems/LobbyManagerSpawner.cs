using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

namespace TryhardParty
{
    public class LobbyManagerSpawner : NetworkBehaviour, IPlayerJoined, IPlayerLeft
    {
        [SerializeField] private NetworkPrefabRef lobbyManagerTestPrefab;

        public void PlayerJoined(PlayerRef player)
        {
            StartCoroutine(WaitForUpdate(player));
        }

        // Wait to Process User
        public IEnumerator WaitForUpdate(PlayerRef _player)
        {
            yield return new WaitForFixedUpdate();
            Runner.Spawn(lobbyManagerTestPrefab, Vector3.zero, Quaternion.identity, _player);
        }

        public void PlayerLeft(PlayerRef player)
        {
        }
    }
}