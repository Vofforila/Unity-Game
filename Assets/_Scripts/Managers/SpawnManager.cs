using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using PlayerCamera;
using Player;
using Fusion.Addons.Physics;
using Proxy;

namespace Host
{
    public class SpawnManager : NetworkBehaviour
    {
        [Header("Player")]
        [SerializeField] private NetworkPrefabRef playerPrefab;

        [SerializeField] private List<Transform> playerSpawnPoints;

        [Header("Catapult")]
        [SerializeField] private List<Transform> catapultSpawnPoints;

        [SerializeField] private NetworkPrefabRef catapultPrefab;

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
        public Dictionary<PlayerRef, NetworkObject> SpawnNetworkPlayers(int _level)
        {
            Dictionary<PlayerRef, NetworkObject> networkPlayerDictionary = new();
            int i = 0;
            int x = playerSpawnPoints.Count;
            foreach (PlayerRef player in Runner.ActivePlayers)
            {
                if (i == x)
                {
                    i = 0;
                }
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

                                /*case 4:
                                    o.GetComponent<Level4PlayerScript>().Init();
                                    break;*/
                        }
                    }
                    );
                // Add Player to Dictionary
                networkPlayerDictionary.Add(player, networkPlayer);

                // Teleport Player to SpawnLocation
                /* NetworkTransform networkTransform = networkPlayer.GetComponent<NetworkTransform>();
                 networkTransform.Teleport(playerSpawnPoints[i].position, playerSpawnPoints[i].rotation);*/

                NetworkRigidbody3D playerRigidbody = networkPlayer.GetComponent<NetworkRigidbody3D>();
                playerRigidbody.Teleport(playerSpawnPoints[i].position, playerSpawnPoints[i].rotation);

                i++;
            }
            return networkPlayerDictionary;
        }

        public IEnumerator ISpawnCatapults()
        {
            foreach (Transform catapultSpawnPoint in catapultSpawnPoints)
            {
                NetworkObject networkCatapult = Runner.Spawn(catapultPrefab, catapultSpawnPoint.position, catapultSpawnPoint.rotation, inputAuthority: null,
                        (Runner, o) =>
                        {
                            o.GetComponent<CatapultPrefab>().Init();
                        }
                        );

                NetworkTransform networkCatapultTransform = networkCatapult.GetComponent<NetworkTransform>();
                networkCatapultTransform.Teleport(catapultSpawnPoint.position, catapultSpawnPoint.rotation);
                yield return new WaitForSecondsRealtime(10f);
            }
        }
    }
}