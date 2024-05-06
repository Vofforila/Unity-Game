using Data;
using Database;
using Fusion;
using SpecialFunction;
using UnityEngine;

namespace UI
{
    public class GameUIListener : NetworkBehaviour
    {
        [Header("Scriptable")]
        [SerializeField] private LocalData localData;
        [SerializeField] private SpecialFunctions specialFunctions;
        [SerializeField] private Firestore firestore;

        [Networked, Capacity(5), HideInInspector] public NetworkString<_16> Username { get; set; }
        [Networked, Capacity(5), HideInInspector] public NetworkString<_16> Rank { get; set; }
        [Networked, HideInInspector] public int Score { get; set; }

        private GameUIManager gameUIManager;

        private ChangeDetector changeDetector;

        #region Awake & Start

        private void Awake()
        {
            gameUIManager = GameUIManager.Instance;
        }

        private void Start()
        {
            gameUIManager.CreateLocalUI();
        }

        #endregion Awake & Start

        public override void Spawned()
        {
            if (localData.currentLvl == 0)
            {
                if (Object.HasInputAuthority)
                {
                    RPC_SendUsername(firestore.accountFirebase.User);
                    RPC_SendRank(firestore.accountFirebase.Rank);
                }
                if (Object.HasStateAuthority)
                {
                    Score = 0;
                }

                gameUIManager.CreateUI(Object.InputAuthority);

                gameUIManager.UpdateUserName(Object.InputAuthority, Username.ToString());
                gameUIManager.UpdateRank(Object.InputAuthority, Rank.ToString());
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
                    case nameof(Rank):
                        gameUIManager.UpdateRank(Object.InputAuthority, Rank.ToString());
                        break;
                    default:
                        break;
                }
            }
        }

        #region OnChange

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

        #endregion OnChange

        #region RPC

        // RPC used to send player information to the Host
        [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
        private void RPC_SendUsername(string _username)
        {
            Username = _username;
        }

        [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
        private void RPC_SendRank(string _rank)
        {
            Rank = _rank;
        }

        #endregion RPC
    }
}