using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;
using UI;
using System.Linq;
using static UI.GameUIManager;

namespace Host
{
    public class LobbyManagerSpawner : NetworkBehaviour, IPlayerJoined, IPlayerLeft
    {
        [Header("Prefab")]
        [SerializeField] private NetworkPrefabRef lobbyManagerTestPrefab;

        [Header("Mach Statistics")]
        [SerializeField] private GameObject matchStatisticsPanel;
        [SerializeField] private GameObject rankPointsGained;
        [SerializeField] public Transform playersStatisticPanel;

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

        public void ShowStatisticPanel()
        {
            matchStatisticsPanel.SetActive(true);

            foreach (PlayerRef player in Runner.ActivePlayers)
            {
                TMP_Text[] playerStatistics = playersStatisticPanel.GetComponentsInChildren<TMP_Text>();

                Dictionary<PlayerRef, PlayerData> sortedList = Instance.PlayerDictionary.
                 OrderBy(pair => pair.Value.Score).ToDictionary(pair => pair.Key, pair => pair.Value);

                int i = 0;
                int place = 0;
                foreach (var kvp in sortedList)
                {
                    place++;
                    playerStatistics[i++].text = place.ToString();
                    playerStatistics[i++].text = kvp.Value.UserName;
                    playerStatistics[i++].text = kvp.Value.Score.ToString();
                    rankPointsGained.GetComponent<TMP_Text>().text = kvp.Value.Score.ToString();
                }
            }
        }
    }
}