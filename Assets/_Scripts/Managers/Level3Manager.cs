using Data;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using TryhardParty;
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
            EndLevel = 5,
        }

        [Header("Internal")]
        [SerializeField] internal SpawnManager spawnManager;

        [Header("Scriptable")]
        [SerializeField] private LocalData localData;

        [Header("Game")]
        public GameState State;

        [Header("Events")]
        [SerializeField] private UnityEvent playLevel4Event;

        [Networked] public PlayerRef Player { get; set; }
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
            spawnManager.SpawnLocal(false);
        }

        public override void Spawned()
        {
            UpdateGameState(GameState.Loading);
        }

        public void PlayeLevel3Event()
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
            spawnManager.SpawnNetworkPlayers(_level: 3, _isKinematic: true);
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
    }
}