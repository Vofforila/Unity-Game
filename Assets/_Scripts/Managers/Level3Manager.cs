using Data;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.Events;

namespace Host
{
    public class Level3Manager : NetworkBehaviour
    {
        public enum GameState
        {
            Loading = 0,
            StartLevel = 1,
            SpawnEnemies = 2,
            IsPlayerAlive = 3,
            DespawnPlayers = 4,
            DespawnEnemies = 5,
            EndLevel = 6,
        }

        [Header("Internal")]
        [SerializeField] internal SpawnManager spawnManager;

        [Header("Scriptable")]
        [SerializeField] private LocalData localData;

        [Header("Game")]
        public GameState State;

        [Header("Events")]
        [SerializeField] private UnityEvent playLevel4Event;

        [Networked, HideInInspector] public int FinishPlace { get; set; }

        private Dictionary<PlayerRef, NetworkObject> networkPlayerDictionary;

        // Singleton
        [HideInInspector] public static Level3Manager Instance;

        private void Awake()
        {
            Instance = this;
            localData.currentLvl = 3;
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

        public void PlayeLevel3Event()
        {
            Debug.Log("<color=yellow>Callback</color>");
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
                    StartCoroutine(ISpawnEnemies());
                    break;

                case GameState.IsPlayerAlive:
                    StartCoroutine(IIsPlayerAlive());
                    break;

                case GameState.DespawnPlayers:
                    DespawnPlayers();
                    break;

                case GameState.DespawnEnemies:
                    DespawnEnemies();
                    break;

                case GameState.EndLevel:
                    EndLevel();
                    break;
            }
        }

        public void StartLevel()
        {
            networkPlayerDictionary = spawnManager.SpawnNetworkPlayers(_level: 3, _isKinematic: true);
            UpdateGameState(GameState.SpawnEnemies);
        }

        public IEnumerator ISpawnEnemies()
        {
            yield return new WaitForSecondsRealtime(5f);
            spawnManager.ISpawnCatapults();
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
            UpdateGameState(GameState.DespawnEnemies);
        }

        public async void DespawnEnemies()
        {
            await spawnManager.DespawnCatapults();
            UpdateGameState(GameState.EndLevel);
        }

        public void EndLevel()
        {
            playLevel4Event.Invoke();
        }
    }
}