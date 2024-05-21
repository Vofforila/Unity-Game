using Fusion;
using System.Collections;
using UnityEngine;

namespace Host
{
    public class LobbyManager : NetworkBehaviour, IPlayerJoined
    {
        [Header("Prefab")]
        [SerializeField] private NetworkPrefabRef lobbyPlayerManagerPrefab;

        // For test
        private int playerCount;

        private void Awake()
        {
            playerCount = 0;
        }

        public void PlayerJoined(PlayerRef _player)
        {
            StartCoroutine(WaitForUpdate(_player));
        }

        // Wait to Process User
        public IEnumerator WaitForUpdate(PlayerRef _player)
        {
            yield return new WaitForFixedUpdate();
            playerCount++;
            NetworkObject player = Runner.Spawn(lobbyPlayerManagerPrefab, Vector3.zero, Quaternion.identity, _player);
            player.name = "Player " + playerCount;
        }
    }
}