using Fusion;
using System.Collections;
using UnityEngine;
using Data;
using UnityEngine.Events;
using System.Collections.Generic;

namespace Host
{
    public class Level1Manager : NetworkBehaviour
    {
        public enum GameState
        {
            StartLevel = 0,
            PlayerTurn = 1,
            DespawnPlayers = 2,
            EndLevel = 3
        }

        [Header("Internal")]
        [SerializeField] internal SpawnManager spawnManager;

        [Header("Scriptable")]
        [SerializeField] private LocalData localData;

        [Header("Game")]
        public GameState State;

        [Header("Events")]
        [SerializeField] private UnityEvent playerTurnEvent;
        [SerializeField] private UnityEvent playLevel2Event;

        [Networked, HideInInspector] public PlayerRef Player { get; set; }
        [Networked, HideInInspector] public NetworkBool PlayerTurn { get; set; }

        private Dictionary<PlayerRef, NetworkObject> networkPlayerDictionary;

        // Singleton
        [HideInInspector] public static Level1Manager Instance;

        private void Awake()
        {
            Instance = this;
            networkPlayerDictionary = new();
        }

        public void Start()
        {
            spawnManager.SpawnLocal(false);
        }

        public void PlayeLevel1Event()
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
                case GameState.PlayerTurn:
                    StartCoroutine(IPlayerTurn());
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
            networkPlayerDictionary = spawnManager.SpawnNetworkPlayers(_level: 1, _isKinematic: true);
            UpdateGameState(GameState.PlayerTurn);
        }

        public IEnumerator IPlayerTurn()
        {
            yield return new WaitForSecondsRealtime(2f);
            foreach (PlayerRef player in Runner.ActivePlayers)
            {
                Player = player;
                PlayerTurn = true;

                Debug.Log("Event - PlayerTurnEvent");
                playerTurnEvent.Invoke();

                yield return new WaitUntil(() => PlayerTurn == false);
            }
            yield return new WaitForSeconds(5f);
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
            playLevel2Event.Invoke();
        }
    }
}