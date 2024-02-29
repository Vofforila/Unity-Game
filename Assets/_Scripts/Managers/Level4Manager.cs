using Data;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using TryhardParty;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.Events;

namespace Host
{
    // Singleton
    public class Level4Manager : NetworkBehaviour
    {
        public enum GameState
        {
            Loading = 0,
            StartLevel = 1,
            SpawnEnemies = 2,
            IsPlayerAlive = 3,
            DespawnPlayers = 4,
            EndLevel = 5,
        }

        [Header("Internal")]
        [SerializeField] internal SpawnManager spawnManager;

        [Header("Scriptable")]
        [SerializeField] private LocalData localData;

        [Header("Game")]
        public GameState State;

        [Header("Events")]
        [SerializeField] private UnityEvent playLevel3Event;

        [Networked, HideInInspector] public int FinishPlace { get; set; }
        [Networked, HideInInspector] public PlayerRef Player { get; set; }

        private Dictionary<PlayerRef, NetworkObject> networkPlayerDictionary;

        // Singleton
        [HideInInspector] public static Level4Manager Instance;

        [Header("Prefab")]
        [SerializeField]
        private NetworkPrefabRef playerPrefab;

        [SerializeField]
        private NetworkPrefabRef fallingBlockPrefab;

        [SerializeField]
        private NetworkPrefabRef coinPrefab;

        [SerializeField]
        private GameObject cameraPrefab;

        [Header("Events")]
        public UnityEvent loadMainMenuEvent;

        private List<NetworkObject> networkPlayerList;
        private int playersAlive;
        internal FusionManager fusionManager;
        private Transform spawnPoint;
        private List<Transform> spawnPointList;
        public NavMeshSurface navMeshSurface;

        private void Awake()
        {
            Instance = this;
            localData.currentLvl = 4;
            networkPlayerDictionary = new();
        }

        public void Start()
        {
            spawnManager.SpawnLocal(false);
        }

        public override void Spawned()
        {
            UpdateGameState(GameState.Loading);
        }

        public void PlayeLevel2Event()
        {
            Debug.Log("Callback");
            if (Object.HasStateAuthority)
            {
                UpdateGameState(GameState.StartLevel);
            }
        }

        public void UpdateGameState(GameState newState)
        {
            State = newState;
            switch (newState)
            {
                case GameState.Loading:
                    break;
                case GameState.StartLevel:
                    StartLevel();
                    break;
                case GameState.SpawnEnemies:
                    SpawnEnemies();
                    break;
                case GameState.IsPlayerAlive:
                    StartCoroutine(IIsPlayerAlive());
                    break;
                case GameState.DespawnPlayers:
                    DespawnPlayers();
                    break;
                case GameState.EndLevel:
                    EndLevel();
                    break;
            }
        }

        public void StartLevel()
        {
            spawnManager.SpawnNetworkPlayers(_level: 4, _isKinematic: true);
            UpdateGameState(GameState.SpawnEnemies);
        }

        public void SpawnEnemies()
        {
            StartCoroutine(spawnManager.ISpawnCatapults());
            UpdateGameState(GameState.IsPlayerAlive);
        }

        public IEnumerator IIsPlayerAlive()
        {
            FinishPlace = Runner.SessionInfo.PlayerCount;
            yield return new WaitUntil(() => FinishPlace == 0);
            UpdateGameState(GameState.DespawnPlayers);
        }

        public void DespawnPlayers()
        {
            foreach (PlayerRef player in Runner.ActivePlayers)
            {
                Runner.Despawn(networkPlayerDictionary[player]);
            }
            UpdateGameState(GameState.EndLevel);
        }

        public void EndLevel()
        {
        }

        /*
        private void Awake()
        {
            fusionManager = GameObject.Find("FusionManager").GetComponent<FusionManager>();
            navMeshSurface = GameObject.Find("NavMeshManager").GetComponent<NavMeshSurface>();

            GameObject.Instantiate(cameraPrefab, new Vector3(0f, 20f, 0f), Quaternion.Euler(90f, 0f, 0f)).GetComponent<CameraManager>().enabled = false;
            StartCoroutine(TipPanel());
        }

        public void PlayLevel4Event()
        {
            Debug.Log("Callback");
            SpawnPlayers();
        }

        public IEnumerator TipPanel()
        {
            yield return new WaitForSecondsRealtime(5f);
            GameObject.Find("GameTip").SetActive(false);
        }

        public void SpawnPlayers()
        {
            StartCoroutine(PlayLevel4());

            networkPlayerList = new();
            playersAlive = 4;

            // Get SpawnPoints
            spawnPoint = GameObject.Find("SpawnPoints").transform;
            spawnPointList = new();
            for (int i = 0; i < spawnPoint.childCount; i++)
                spawnPointList.Add(spawnPoint.GetChild(i));

            // Spawn Player
            for (int i = 0; i < localData.playerListData.Count; i++)
            {
                PlayerData playerData = localData.playerListData[i];
                NetworkObject networkPlayer = fusionManager.runner.Spawn(playerPrefab, spawnPointList[i].position, Quaternion.identity, playerData.PlayerRef);
                NetworkTransform networkTransform = networkPlayer.GetComponent<NetworkTransform>();

                networkTransform.gameObject.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
                networkTransform.Teleport(spawnPointList[i].position);

                networkPlayerList.Add(networkPlayer);
            }

            StartCoroutine(SpawnStone());
        }

        public void DestroyPlayerEvent()
        {
            Debug.Log("Callback");
            for (int i = 0; i < localData.playerListData.Count; i++)
            {
                PlayerData playerData = localData.playerListData[i];
                if (playerData.Username == firestore.accountFirebase.User)
                {
                    fusionManager.runner.Despawn(networkPlayerList[i]);
                    playersAlive--;
                    break;
                }
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

        private IEnumerator SpawnStone()
        {
            // Create a array that stores the stone positions
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
                    NetworkObject networkCoin = fusionManager.runner.Spawn(coinPrefab, new Vector3(fallingBlockArray[coinPosition, 0], 0.8f, fallingBlockArray[coinPosition, 1]), Quaternion.Euler(-90f, -180f, 0f));
                }

                // Spawn Falling block
                NetworkObject networkFallingBlock = fusionManager.runner.Spawn(fallingBlockPrefab, new Vector3(fallingBlockArray[i, 0], 1.1f, fallingBlockArray[i, 1]), Quaternion.identity);

                // Update NavAgent
                navMeshSurface.BuildNavMesh();

                // Wait for block to fall
                yield return new WaitForFixedUpdate();
                networkFallingBlock.GetComponent<BoxCollider>().isTrigger = false;

                // Repeat
                yield return new WaitForSecondsRealtime(6f);
            }
        }

        // Wait for players
        public IEnumerator PlayLevel4()
        {
            yield return new WaitUntil(() => playersAlive == 0);
            Debug.Log("Load MainMenu - Event");
            loadMainMenuEvent.Invoke();
        }
        */
    }
}