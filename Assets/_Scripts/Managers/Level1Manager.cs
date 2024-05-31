using Data;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.Events;

namespace Host
{
    public class Level1Manager : NetworkBehaviour
    {
        public enum GameState
        {
            Loading = 0,
            StartLevel = 1,
            PlayerTurn = 2,
            DespawnPlayers = 3,
            EndLevel = 4,
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
        [Networked, HideInInspector] private NetworkBool StartLights { get; set; }

        private Dictionary<PlayerRef, NetworkObject> networkPlayerDictionary;

        private ChangeDetector changeDetector;

        // Singleton
        [HideInInspector] public static Level1Manager Instance;

        private void Awake()
        {
            Instance = this;
            networkPlayerDictionary = new();
        }

        public void Start()
        {
            spawnManager.SpawnLocal();
            localData.currentLvl = 1;
            GameUIManager.Instance.UpdateLevelState(localData.currentLvl);
            UpdateGameState(GameState.Loading);
        }

        public override void Spawned()
        {
            StartLights = false;
            changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
        }

        public override void Render()
        {
            foreach (var change in changeDetector.DetectChanges(this, out var previousBuffer, out var currentBuffer))
            {
                switch (change)
                {
                    case nameof(StartLights):
                        if (StartLights == true)
                        {
                            StartCoroutine(LightManager.Instance.SetupLights());
                        }
                        break;
                }
            }
        }

        public void PlayeLevel1Event()
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
                case GameState.PlayerTurn:
                    StartCoroutine(IPlayerTurn());
                    break;
                case GameState.DespawnPlayers:
                    DespawnPlayers();
                    break;
                case GameState.EndLevel:
                    EndLevel();
                    break;
                default:
                    break;
            }
        }

        public void StartLevel()
        {
            networkPlayerDictionary = spawnManager.SpawnNetworkPlayers(_level: 1, _isKinematic: true);
            StartLights = true;
            UpdateGameState(GameState.PlayerTurn);
        }

        public IEnumerator IPlayerTurn()
        {
            yield return new WaitForSecondsRealtime(4f);
            foreach (PlayerRef player in Runner.ActivePlayers)
            {
                Player = player;
                PlayerTurn = true;

                Debug.Log("<color=yellow>Event - PlayerTurnEvent</color>");
                playerTurnEvent.Invoke();

                yield return new WaitUntil(() => PlayerTurn == false);
            }

            Player = PlayerRef.None;
            PlayerTurn = false;
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