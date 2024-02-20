using Data;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Host
{
    // SingleTon
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

        [Networked] public int FinishPlace { get; set; }
        [Networked] public PlayerRef Player { get; set; }

        private Dictionary<PlayerRef, NetworkObject> networkPlayerDictionary;

        public GameState State;

        public static Level2Manager Instance;

        private ChangeDetector changeDetector;

        private void Awake()
        {
            Instance = this;
            networkPlayerDictionary = new();
        }

        public void Start()
        {
            spawnManager.SpawnLocal(false);
        }

        public override void Spawned()
        {
            changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
        }

        public override void Render()
        {
            foreach (var change in changeDetector.DetectChanges(this, out var previousBuffer, out var currentBuffer))
            {
                switch (change)
                {
                    case nameof(Player):
                        AddScore();
                        break;
                }
            }
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

        public void AddScore()
        {
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
            UpdateGameState(GameState.Racing);
        }

        public IEnumerator IRacing()
        {
            FinishPlace = 4;
            yield return new WaitUntil(() => FinishPlace != 0);
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
            /*  playLevel3Event.Invoke();*/
        }
    }
}