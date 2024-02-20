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
            StartLevel = 0,
            SpawnEnemies = 1,
            IsPlayerAlive = 2,
            DespawnPlayers = 3,
            EndLevel = 4
        }

        [Header("Internal")]
        [SerializeField] internal SpawnManager spawnManager;
        [SerializeField] private LocalData localData;

        [Header("Events")]
        [SerializeField] private UnityEvent playLevel4Event;

        private Dictionary<PlayerRef, NetworkObject> networkPlayerDictionary;

        public GameState State;

        public static Level3Manager Instance;

        private void Awake()
        {
            Instance = this;
            networkPlayerDictionary = new();
        }

        public void Start()
        {
            spawnManager.SpawnLocal(false);
        }

        public void PlayeLevel3Event()
        {
            Debug.Log("Callback");
            if (Object.HasStateAuthority)
            {
                localData.currentLvl = 1;
                UpdateGameState(GameState.StartLevel);
            }
        }

        public void UpdateGameState(GameState newState)
        {
            State = newState;
            switch (newState)
            {
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
            networkPlayerDictionary = spawnManager.SpawnNetworkPlayers(3);
            UpdateGameState(GameState.SpawnEnemies);
        }

        public void SpawnEnemies()
        {
            StartCoroutine(spawnManager.ISpawnCatapults());
            UpdateGameState(GameState.IsPlayerAlive);
        }

        public IEnumerator IIsPlayerAlive()
        {
            yield return null;

            UpdateGameState(GameState.DespawnPlayers);
        }

        public void DespawnPlayers()
        {
            UpdateGameState(GameState.EndLevel);
        }

        public void EndLevel()
        {
        }
    }
}