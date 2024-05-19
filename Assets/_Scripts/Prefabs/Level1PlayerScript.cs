using Data;
using Fusion;
using Host;
using PlayerInput;
using SpecialFunction;
using UI;
using UnityEngine;

namespace Player
{
    public class Level1PlayerScript : NetworkBehaviour
    {
        [Header("Scriptable")]
        [SerializeField] private LocalData localData;

        [Header("Internal")]
        [SerializeField] internal PlayerVisuals playerVisuals;
        [SerializeField] internal GameUIListener gameUIListener;
        [HideInInspector] internal QuadraticCurve curve;

        [Networked] private NetworkBool IsJumping { get; set; }
        [Networked] private PlayerRef CurrentPlayerTurn { get; set; }
        [Networked] private NetworkButtons ButtonsPrevious { get; set; }

        private ChangeDetector changeDetector;

        [Header("Read Only")]
        [Header("Object")]
        [SerializeField] private float size = 0.8f;
        [SerializeField] private bool isKinematic = true;
        [SerializeField] private bool[] constrains = null;
        [SerializeField] private float mass = 0f;

        [Header("Game")]
        private const float speed = 0.8f;

        private float sampleTime;
        private int jumpPosition;
        private int score;
        private Vector3 newPosition;
        private Vector3 newRotation;

        private void Awake()
        {
            if (localData.currentLvl != 1)
            {
                enabled = false;
            }
        }

        // Start before Sync
        public void Init()
        {
            if (localData.currentLvl == 1)
            {
                // Set Game Values
                sampleTime = 2f;
                jumpPosition = 0;
                score = 0;

                // Set Network Values
                IsJumping = false;

                // Objects
                curve = GameObject.Find("Curve").GetComponent<QuadraticCurve>();
            }
        }

        public override void Spawned()
        {
            // Disable player
            if (localData.currentLvl == 1)
            {
                playerVisuals.SetPlayer(_visuals: false, _size: size, _isKinematic: isKinematic, _constrains: constrains, _mass: mass);
                changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
            }
        }

        // Only runs Input check on Client rest is done by Host
        public override void FixedUpdateNetwork()
        {
            if (Level1Manager.Instance.Player == Object.InputAuthority)
            {
                if (GetInput<NetworkInputData>(out var inputData))
                {
                    // compute pressed/released state
                    NetworkButtons pressed = inputData.GameButton.GetPressed(ButtonsPrevious);
                    NetworkButtons released = inputData.GameButton.GetReleased(ButtonsPrevious);

                    // store latest input as 'previous' state we had
                    ButtonsPrevious = inputData.GameButton;

                    if (pressed.IsSet(GameButton.Z) && IsJumping == false)
                    {
                        if (Object.HasStateAuthority)
                        {
                            Debug.Log("<color=green>Jump</color>");
                            Jump();
                        }
                    }
                    if (pressed.IsSet(GameButton.X) && IsJumping == false && score != 0)
                    {
                        if (Object.HasStateAuthority)
                        {
                            Debug.Log("<color=green>Take Money</color>");
                            TakeMoney();
                        }
                    }
                }
            }

            // Handle Movement
            if (Object.HasStateAuthority)
            {
                MakePlayerJump();
            }
        }

        public override void Render()
        {
            foreach (var change in changeDetector.DetectChanges(this, out var previousBuffer, out var currentBuffer))
            {
                switch (change)
                {
                    case nameof(CurrentPlayerTurn):
                        playerVisuals.SetVisuals(CurrentPlayerTurn == Object.InputAuthority);
                        break;
                }
            }
        }

        private void PlayerTurnEvent()
        {
            Debug.Log("<color=yellow>Callback</color>");
            QuadraticCurveManager.Instance.A.position = QuadraticCurveManager.Instance.startPosition.position;
            QuadraticCurveManager.Instance.B.position = QuadraticCurveManager.Instance.startPosition.position;
            CurrentPlayerTurn = Level1Manager.Instance.Player;
        }

        private void MakePlayerJump()
        {
            // Move the player from point A to B
            if (sampleTime <= 1f)
            {
                // Calculate Trajectory
                sampleTime += Runner.DeltaTime * speed;
                newPosition = curve.Evaluate(sampleTime);
                newRotation = curve.Evaluate(sampleTime + 0.001f) - transform.position;

                // Update Player
                if (newRotation != Vector3.zero)
                {
                    transform.position = newPosition;
                    transform.forward = newRotation;
                }
                if (sampleTime >= 1)
                {
                    // Reset Player rotation
                    transform.rotation = Quaternion.Euler(0f, 90f, 0f);
                    transform.position = newPosition;
                    IsJumping = false;

                    // Check to see if player got 1st place or died
                    if (score == 500)
                    {
                        TakeMoney();
                    }
                    else if (score == 0)
                    {
                        TakeMoney();
                    }
                }
            }
        }

        private void Jump()
        {
            // Generate a random number to where you jump
            jumpPosition = Random.Range(jumpPosition, 8);
            Debug.Log(jumpPosition);
            IsJumping = true;
            score = QuadraticCurveManager.Instance.MoveCurve(jumpPosition);
            RPC_JumpSound();
            sampleTime = 0f;
        }

        private void TakeMoney()
        {
            gameUIListener.AddScore(score);
            Level1Manager.Instance.PlayerTurn = false;
        }

        [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
        private void RPC_JumpSound()
        {
            SoundManager.Instance.PlaySound("jump-sound");
        }
    }
}