using UnityEngine;
using Fusion;
using Data;
using SpecialFunction;
using Database;

namespace UI
{
    public class GameUIListener : NetworkBehaviour
    {
        [Header("Scriptable")]
        [SerializeField] private LocalData localData;
        [SerializeField] private SpecialFunctions specialFunctions;
        [SerializeField] private Firestore firestore;

        [Networked, Capacity(5), HideInInspector] public NetworkString<_16> Username { get; set; }
        [Networked, HideInInspector] public int Score { get; set; }

        private GameUIManager gameUIManager;

        private ChangeDetector changeDetector;

        private void Awake()
        {
            gameUIManager = GameUIManager.Instance;
        }

        private void Start()
        {
            gameUIManager.CreateLocalUI();
        }

        public override void Spawned()
        {
            gameUIManager.UpdateLevelState(localData.currentLvl);
            if (localData.currentLvl == 4)
            {
                if (Object.HasInputAuthority)
                {
                    RPC_SendUsername(firestore.accountFirebase.User);
                }
                if (Object.HasStateAuthority)
                {
                    Score = 0;
                }

                gameUIManager.CreateUI(Object.InputAuthority);

                gameUIManager.UpdateUserName(Object.InputAuthority, Username.ToString());
                gameUIManager.UpdateScore(Object.InputAuthority, 0);
            }

            changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
        }

        public override void Render()
        {
            foreach (var change in changeDetector.DetectChanges(this, out var previousBuffer, out var currentBuffer))
            {
                switch (change)
                {
                    case nameof(Score):
                        gameUIManager.UpdateScore(Object.InputAuthority, Score);
                        break;
                    case nameof(Username):
                        gameUIManager.UpdateUserName(Object.InputAuthority, Username.ToString());
                        break;
                    default:
                        break;
                }
            }
        }

        public void AddScore(int _score)
        {
            Score += _score;
            Debug.Log(_score);
        }

        public void UpdateHp(int _hp)
        {
            gameUIManager.UpdateHp(_hp);
            Debug.Log(_hp);
        }

        // RPC used to send player information to the Host
        [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
        private void RPC_SendUsername(string _username)
        {
            Username = _username;
        }
    }
}