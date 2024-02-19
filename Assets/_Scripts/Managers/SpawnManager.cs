using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using PlayerCamera;
using Player;

namespace Host
{
    public class SpawnManager : NetworkBehaviour
    {
        [Header("Player")]
        [SerializeField] private NetworkPrefabRef playerPrefab;
        [SerializeField] private List<Transform> spawnPoints;

        [Header("Camera")]
        [SerializeField] private GameObject playerCameraPrefab;
        [SerializeField] private Transform cameraSpawnPoint;

        // Local
        public void SpawnLocal(bool _enableCamera)
        {
            GameObject playerCamera = Instantiate(playerCameraPrefab, cameraSpawnPoint.position, cameraSpawnPoint.rotation);
            if (_enableCamera == false)
            {
                playerCamera.GetComponent<CameraManager>().enabled = false;
            }
        }

        // Host
        public Dictionary<PlayerRef, NetworkObject> SpawnNetwork(int _level)
        {
            Dictionary<PlayerRef, NetworkObject> networkPlayerDictionary = new();
            int i = 0;
            int x = spawnPoints.Count;
            foreach (PlayerRef player in Runner.ActivePlayers)
            {
                if (i == x)
                {
                    i = 0;
                }
                // Spawn Player
                NetworkObject networkPlayer = Runner.Spawn(playerPrefab, spawnPoints[i].position, spawnPoints[i].rotation, player,
                    (Runner, o) =>
                    {
                        switch (_level)
                        {
                            case 1:
                                o.GetComponent<Level1PlayerScript>().Init();
                                break;
                            case 2:
                                o.GetComponent<Level2PlayerScript>().Init();
                                break;
                                /*
                                 case 3:
                                     o.GetComponent<Level3PlayerScript>().Init();
                                     break;
                                 case 4:
                                     o.GetComponent<Level4PlayerScript>().Init();
                                     break;*/
                        }
                    }
                    );
                // Add Player to Dictionary
                networkPlayerDictionary.Add(player, networkPlayer);

                // Teleport Player to SpawnLocation
                NetworkTransform networkTransform = networkPlayer.GetComponent<NetworkTransform>();
                networkTransform.Teleport(spawnPoints[i].position, spawnPoints[i].rotation);

                i++;
            }
            return networkPlayerDictionary;
        }
    }
}