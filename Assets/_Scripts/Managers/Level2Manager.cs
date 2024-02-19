using Data;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Host
{
    public class Level2Manager : NetworkBehaviour
    {
        public enum GameState
        {
            StartLevel = 0,
            Racing = 1,
            DespawnPlayers = 2,
            EndLevel = 3
        }

        [Header("Internal")]
        [SerializeField] internal SpawnManager spawnManager;
        [SerializeField] private LocalData localData;

        [Header("Events")]
        [SerializeField] private UnityEvent playLevel3Event;

        private Dictionary<PlayerRef, NetworkObject> networkPlayerDictionary;

        public GameState State;

        public static Level2Manager Instance;

        private void Awake()
        {
            Instance = this;
            networkPlayerDictionary = new();
        }

        public void Start()
        {
            spawnManager.SpawnLocal(false);
        }

        public void PlayeLevel2Event()
        {
            Debug.Log("Callback");
            if (Object.HasStateAuthority)
            {
                localData.currentLvl = 2;
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
                case GameState.Racing:
                    StartCoroutine(IRacing());
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
            networkPlayerDictionary = spawnManager.SpawnNetwork(2);
            /* UpdateGameState(GameState.Racing);*/
        }

        public IEnumerator IRacing()
        {
            yield return new WaitForEndOfFrame();
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
            /*  playLevel3Event.Invoke();*/
        }
    }
}