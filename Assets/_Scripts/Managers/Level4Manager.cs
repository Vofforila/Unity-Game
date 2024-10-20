using Data;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UI;
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
        [SerializeField] private UnityEvent loadMainMenuEvent;

        [Networked, HideInInspector] public int FinishPlace { get; set; }

        private Dictionary<PlayerRef, NetworkObject> networkPlayerDictionary;

        // Singleton
        public static Level4Manager Instance;

        private void Awake()
        {
            Instance = this;
            localData.currentLvl = 4;
            networkPlayerDictionary = new();
        }

        public void Start()
        {
            spawnManager.SpawnLocal();
            GameUIManager.Instance.UpdateLevelState(localData.currentLvl);
        }

        public override void Spawned()
        {
            UpdateGameState(GameState.Loading);
        }

        public void PlayLevel4Event()
        {
            Debug.Log("<color=yellow>Callback</color>");
            if (Object.HasStateAuthority)
            {
                UpdateGameState(GameState.StartLevel);
            }
        }

        public async void UpdateGameState(GameState newState)
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
                    StartCoroutine(SpawnEnemies());
                    break;
                case GameState.IsPlayerAlive:
                    StartCoroutine(IIsPlayerAlive());
                    break;
                case GameState.DespawnPlayers:
                    DespawnPlayers();
                    break;
                case GameState.EndLevel:
                    await EndLevelAsync();
                    break;
            }
        }

        public void StartLevel()
        {
            networkPlayerDictionary = spawnManager.SpawnNetworkPlayers(_level: 4, _isKinematic: true);
            UpdateGameState(GameState.SpawnEnemies);
        }

        public IEnumerator SpawnEnemies()
        {
            yield return new WaitForSecondsRealtime(5f);
            StartCoroutine(spawnManager.ISpawnFallingBlox());
            UpdateGameState(GameState.IsPlayerAlive);
        }

        public IEnumerator IIsPlayerAlive()
        {
            FinishPlace = Runner.SessionInfo.PlayerCount;
            yield return new WaitUntil(() => FinishPlace == 0);
            yield return new WaitForSecondsRealtime(5f);
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

        public async Task EndLevelAsync()
        {
            RPC_ShowMainMenu();
            await Task.Delay(5000);
            loadMainMenuEvent.Invoke();
        }

        [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
        private void RPC_ShowMainMenu()
        {
            localData.currentLvl = -1;
        }
    }
}