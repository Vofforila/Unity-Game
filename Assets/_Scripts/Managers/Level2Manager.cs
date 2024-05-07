using Data;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.Events;

namespace Host
{
    // SingleTon
    public class Level2Manager : NetworkBehaviour
    {
        public enum GameState
        {
            Loading = 0,
            StartLevel = 1,
            Racing = 2,
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
        [SerializeField] private UnityEvent playLevel3Event;

        [Networked, HideInInspector] public int FinishPlace { get; set; }
        [Networked, HideInInspector] public PlayerRef Player { get; set; }

        private Dictionary<PlayerRef, NetworkObject> networkPlayerDictionary;

        // Singleton
        public static Level2Manager Instance;

        private void Awake()
        {
            Instance = this;
            localData.currentLvl = 2;
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

        public void PlayeLevel2Event()
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
                    StartCoroutine(IStartLevel());
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

        public IEnumerator IStartLevel()
        {
            yield return new WaitForSecondsRealtime(2f);
            SoundManager.Instance.PlaySound("race-beep-1");
            yield return new WaitForSecondsRealtime(0.601f);
            SoundManager.Instance.PlaySound("race-beep-2");
            yield return new WaitForSecondsRealtime(0.627f);
            SoundManager.Instance.PlaySound("race-beep-3");
            yield return new WaitForSecondsRealtime(0.601f);
            SoundManager.Instance.PlaySound("gun-shot-shound");
            SoundManager.Instance.PlaySound("running-sound");

            networkPlayerDictionary = spawnManager.SpawnNetworkPlayers(_level: 2, _isKinematic: true);
            UpdateGameState(GameState.Racing);
        }

        public IEnumerator IRacing()
        {
            FinishPlace = Runner.SessionInfo.PlayerCount;
            yield return new WaitUntil(() => FinishPlace == 0);
            yield return new WaitForEndOfFrame();
            UpdateGameState(GameState.DespawnPlayers);
        }

        public void DespawnPlayers()
        {
            foreach (PlayerRef player in Runner.ActivePlayers)
            {
                Runner.Despawn(networkPlayerDictionary[player]);
            }
            RPC_StopSound();
            UpdateGameState(GameState.EndLevel);
        }

        public void EndLevel()
        {
            playLevel3Event.Invoke();
        }

        [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
        private void RPC_StopSound()
        {
            SoundManager.Instance.StopSound("running-sound");
        }
    }
}