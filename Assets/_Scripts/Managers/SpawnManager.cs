using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Player;
using Fusion.Addons.Physics;
using Unity.AI.Navigation;

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

        [Header("Falling Blocks")]
        [SerializeField] private NetworkPrefabRef fallingBlockPrefab;
        [SerializeField] private NetworkPrefabRef coinPrefab;

        [Header("Nav Mesh Agent")]
        [SerializeField] private NavMeshSurface navMesh;

        [Header("Camera")]
        [SerializeField] private GameObject playerCameraPrefab;

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
                i++;
                Debug.Log(x);
                Debug.Log(i);
                if (i == x)
                {
                    i = 1;
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

                            case 4:
                                o.GetComponent<Level4PlayerScript>().Init();
                                break;
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
            }
            return networkPlayerDictionary;
        }

        public IEnumerator ISpawnCatapults()
        {
            Transform[] catapultSpawnPoints = GameObject.Find("CatapultSpawnPoints").GetComponentsInChildren<Transform>();
            for (int i = 1; i <= catapultSpawnPoints.Length - 1; i++)
            {
                NetworkObject networkCatapult = Runner.Spawn(catapultPrefab, catapultSpawnPoints[i].position, catapultSpawnPoints[i].rotation);

                NetworkTransform networkCatapultTransform = networkCatapult.GetComponent<NetworkTransform>();
                networkCatapultTransform.Teleport(catapultSpawnPoints[i].position, catapultSpawnPoints[i].rotation);
                yield return new WaitForSecondsRealtime(1f);
            }
        }

        public IEnumerator ISpawnFallingBlox()
        {
            int mapSize = 100;
            float[,] fallingBlockArray = new float[mapSize, 2];

            int count = 0;
            for (float z = -9f; z <= 9; z += 2)
            {
                for (float x = -9f; x <= 9; x += 2)
                {
                    fallingBlockArray[count, 0] = x;
                    fallingBlockArray[count, 1] = z;
                    count++;
                }
            }

            // Shuffle the array
            Shuffle2DArray(fallingBlockArray);

            int coinPosition = 0;
            // Spawn the stones at random positions
            for (int i = 0; i < mapSize; i++)
            {
                if (i % 5 == 0)
                {
                    coinPosition = i + Random.Range(10, 20);
                    if (coinPosition >= 100)
                        coinPosition = 100;

                    // Spawn Falling block
                    NetworkObject networkCoin = Runner.Spawn(coinPrefab, new Vector3(fallingBlockArray[coinPosition, 0], 0.8f, fallingBlockArray[coinPosition, 1]), Quaternion.identity);
                    NetworkTransform networkCoinTransform = networkCoin.GetComponent<NetworkTransform>();
                    networkCoinTransform.Teleport(new Vector3(fallingBlockArray[coinPosition, 0], 0.8f, fallingBlockArray[coinPosition, 1]), Quaternion.identity);
                }

                // Spawn Falling block
                NetworkObject networkFallingBlock = Runner.Spawn(fallingBlockPrefab, new Vector3(fallingBlockArray[i, 0], 1.1f, fallingBlockArray[i, 1]), Quaternion.identity);
                NetworkTransform networkFallingBlockTransform = networkFallingBlock.GetComponent<NetworkTransform>();
                networkFallingBlockTransform.Teleport(new Vector3(fallingBlockArray[i, 0], 1.1f, fallingBlockArray[i, 1]), Quaternion.identity);

                // Wait for block to fall
                yield return new WaitForFixedUpdate();
                networkFallingBlock.GetComponent<BoxCollider>().isTrigger = false;

                // Repeat
                yield return new WaitForSecondsRealtime(6f);
            }
        }

        private void Shuffle2DArray<T>(T[,] array)
        {
            int rowCount = array.GetLength(0);

            for (int i = 0; i < rowCount; i++)
            {
                int randomIndex = Random.Range(i, rowCount);

                // Swap rows i and randomIndex
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    T temp = array[i, j];
                    array[i, j] = array[randomIndex, j];
                    array[randomIndex, j] = temp;
                }
            }
        }
    }
}