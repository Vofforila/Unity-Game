using Data;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Singleton;
using TryhardParty;
using Fusion.Addons.Physics;
using UI;
using Host;

namespace Player
{
    public class Level2PlayerScript : NetworkBehaviour
    {
        [Header("Scriptable")]
        [SerializeField] private LocalData localData;
        [SerializeField] private Firestore firestore;

        [Header("Internal")]
        [SerializeField] internal PlayerVisuals playerVisuals;
        [SerializeField] internal GameUIListener gameUIListener;

        [Networked] private TickTimer KeyCooldown { get; set; }
        [Networked] private float HorseSpeed { get; set; }
        [Networked] public NetworkButtons ButtonsPrevious { get; set; }
        [Networked] private PlayerRef WinningPlayer { get; set; }
        private ChangeDetector changeDetector;

        [Header("Read Only")]
        [SerializeField] private Vector3 size = new(2f, 2f, 2f);
        [SerializeField] private bool[] constrains = { false, false, false, true, true, true };
        [SerializeField] private int score;

        private void Awake()
        {
            if (localData.currentLvl != 2)
            {
                this.enabled = false;
            }
        }

        public void Init()
        {
            if (localData.currentLvl == 2)
            {
                WinningPlayer = PlayerRef.None;
                Debug.Log("Init");
                HorseSpeed = 1f;
                KeyCooldown = TickTimer.CreateFromSeconds(Runner, 0.2f);
            }
        }

        public override void Spawned()
        {
            if (localData.currentLvl == 2)
            {
                EnablePlayer(true);
            }
            changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
        }

        // Client
        public override void FixedUpdateNetwork()
        {
            // Handle Input
            if (GetInput<NetworkInputData>(out var inputData))
            {
                // compute pressed/released state
                NetworkButtons pressed = inputData.GameButton.GetPressed(ButtonsPrevious);
                NetworkButtons released = inputData.GameButton.GetReleased(ButtonsPrevious);

                // store latest input as 'previous' state we had
                ButtonsPrevious = inputData.GameButton;

                if (pressed.IsSet(GameButton.Z) && KeyCooldown.Expired(Runner) == true)
                {
                    HorseSpeed++;
                    KeyCooldown = TickTimer.CreateFromSeconds(Runner, 0.2f);
                }
            }
            // Handle Movement
            MakeHorseRun();
        }

        public override void Render()
        {
            foreach (var change in changeDetector.DetectChanges(this, out var previousBuffer, out var currentBuffer))
            {
                switch (change)
                {
                    case nameof(WinningPlayer):
                        Debug.Log("Run");
                        EnablePlayer(false);
                        break;
                }
            }
        }

        public void EnablePlayer(bool _var)
        {
            // Enable Visuals
            playerVisuals.SetVisuals(_var);
            playerVisuals.SetSize(size);
            /*  playerVisuals.SetRigidbody(true, constrains, 10);*/
        }

        public void MakeHorseRun()
        {
            transform.position += HorseSpeed * Runner.DeltaTime * transform.forward;
        }

        // Check Collision
        public void OnTriggerEnter(Collider other)
        {
            // Update score
            if (Object.HasInputAuthority && other.gameObject.CompareTag("Finish"))
            {
                Debug.Log("Collision");
                RPC_PlayerFinished(Object.InputAuthority);
            }
        }

        [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
        private void RPC_PlayerFinished(PlayerRef _player)
        {
            if (Level2Manager.Instance.FinishPlace == 1)
            {
                score = 250;
                gameUIListener.AddScore(score);
            }
            if (Level2Manager.Instance.FinishPlace == 2)
            {
                score = 350;
                gameUIListener.AddScore(score);
            }
            if (Level2Manager.Instance.FinishPlace == 3)
            {
                score = 400;
                gameUIListener.AddScore(score);
            }
            if (Level2Manager.Instance.FinishPlace == 4)
            {
                score = 500;
                gameUIListener.AddScore(score);
            }

            WinningPlayer = _player;
            Level2Manager.Instance.FinishPlace--;
        }
    }
}