using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using PlayerCamera;
using Player;
using Fusion.Addons.Physics;

namespace Host
{
    public class SpawnManager : NetworkBehaviour
    {
        [Header("Player")]
        private NetworkPrefabRef playerPrefab;
        [SerializeField] private NetworkPrefabRef playerPrefabR;
        [SerializeField] private NetworkPrefabRef playerPrefabT;

        [Header("Catapult")]
        [SerializeField] private NetworkPrefabRef catapultPrefab;

        [Header("Camera")]
        [SerializeField] private GameObject playerCameraPrefab;

        private static NetworkObjectBaker _baker;
        private static NetworkObjectBaker Baker => _baker ??= new NetworkObjectBaker();

        // Local
        public void SpawnLocal(bool _enableCamera)
        {
            Transform cameraSpawnPoint = GameObject.Find("CameraSpawnPoint").GetComponent<Transform>();
            GameObject playerCamera = Instantiate(playerCameraPrefab, cameraSpawnPoint.position, cameraSpawnPoint.rotation);
            if (_enableCamera == false)
            {
                playerCamera.GetComponent<CameraManager>().enabled = false;
            }
        }

        // Host
        public Dictionary<PlayerRef, NetworkObject> SpawnNetworkPlayers(int _level, bool _isKinematic)
        {
            Dictionary<PlayerRef, NetworkObject> networkPlayerDictionary = new();

            // Get SpawnPoints
            Transform[] playerSpawnPoints = GameObject.Find("SpawnPoints").GetComponentsInChildren<Transform>();

            // Spawn Every Player on a Spawnpoint
            int i = 0;
            int x = playerSpawnPoints.Length;
            foreach (PlayerRef player in Runner.ActivePlayers)
            {
                if (i == x)
                {
                    i = 0;
                }

                if (_isKinematic)
                {
                    playerPrefab = playerPrefabT;
                }
                else playerPrefab = playerPrefabR;

                // Spawn Player
                NetworkObject networkPlayer = Runner.Spawn(playerPrefab, playerSpawnPoints[i].position, playerSpawnPoints[i].rotation, player,
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

                            case 3:
                                o.GetComponent<Level3PlayerScript>().Init();
                                break;

                                /* case 4:
                                     o.GetComponent<Level4PlayerScript>().Init();
                                     break;*/
                        }
                    });

                // Add Player to Dictionary
                networkPlayerDictionary.Add(player, networkPlayer);

                // Teleport Player to SpawnLocation
                if (_isKinematic)
                {
                    NetworkTransform playerTransform = networkPlayer.GetComponent<NetworkTransform>();
                    playerTransform.Teleport(playerSpawnPoints[i].position, playerSpawnPoints[i].rotation);
                }
                else
                {
                    NetworkRigidbody3D playerRigidbody = networkPlayer.GetComponent<NetworkRigidbody3D>();
                    playerRigidbody.Teleport(playerSpawnPoints[i].position, playerSpawnPoints[i].rotation);
                }

                i++;
            }
            return networkPlayerDictionary;
        }

        public IEnumerator ISpawnCatapults()
        {
            Transform[] catapultSpawnPoints = GameObject.Find("CatapultSpawnPoints").GetComponentsInChildren<Transform>();
            foreach (Transform catapultSpawnPoint in catapultSpawnPoints)
            {
                NetworkObject networkCatapult = Runner.Spawn(catapultPrefab, catapultSpawnPoint.position, catapultSpawnPoint.rotation);

                NetworkTransform networkCatapultTransform = networkCatapult.GetComponent<NetworkTransform>();
                networkCatapultTransform.Teleport(catapultSpawnPoint.position, catapultSpawnPoint.rotation);
                yield return new WaitForSecondsRealtime(10f);
            }
        }
    }
}