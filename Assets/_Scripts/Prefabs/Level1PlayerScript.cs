using Data;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Singleton;
using UI;
using Host;
using System;

namespace Player
{
    public class Level1PlayerScript : NetworkBehaviour
    {
        [Header("Scriptable")]
        [SerializeField] private LocalData localData;

        [SerializeField] private LocalInputData localInputData;
        [SerializeField] private ServerData serverData;

        [Header("Internal")]
        [SerializeField] internal PlayerVisuals playerVisuals;

        [SerializeField] internal GameUIListener gameUIListener;
        [HideInInspector] internal QuadraticCurve curve;

        [Networked] private NetworkBool IsJumping { get; set; }
        [Networked] private PlayerRef CurrentPlayerTurn { get; set; }
        [Networked] private NetworkButtons ButtonsPrevious { get; set; }

        private ChangeDetector changeDetector;

        [Header("Read Only")]
        [SerializeField] private float speed = 0.8f;

        [SerializeField] private Vector3 size = new(1f, 1f, 1f);
        [SerializeField] private int score;
        [SerializeField] private int jumpPosition;
        private Vector3 NewPosition;
        private Vector3 NewRotation;
        private float sampleTime;

        private void Awake()
        {
            if (localData.currentLvl != 1)
            {
                this.enabled = false;
            }
        }

        // Start before Sync
        public void Init()
        {
            curve = GameObject.Find("Curve").GetComponent<QuadraticCurve>();
            sampleTime = 2f;
            jumpPosition = 0;
            IsJumping = false;
            score = 0;
            EnablePlayer(false);
        }

        // Disable player
        public override void Spawned()
        {
            changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
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
                        Debug.Log("Jump");
                        Jump();
                    }
                    if (pressed.IsSet(GameButton.X) && IsJumping == false && score != 0)
                    {
                        Debug.Log("Take Money");
                        TakeMoney();
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
                        EnablePlayer(CurrentPlayerTurn == Object.InputAuthority);
                        break;
                }
            }
        }

        public void PlayerTurnEvent()
        {
            Debug.Log("Callback");
            QuadraticCurveManager.Instance.A.position = QuadraticCurveManager.Instance.startPosition.position;
            QuadraticCurveManager.Instance.B.position = QuadraticCurveManager.Instance.startPosition.position;
            CurrentPlayerTurn = Level1Manager.Instance.Player;
        }

        public void EnablePlayer(bool _var)
        {
            // Enable Visuals
            playerVisuals.SetVisuals(_var);
            playerVisuals.SetSize(size);
        }

        public void MakePlayerJump()
        {
            // Move the player from point A to B

            if (sampleTime <= 1f)
            {
                // Calculate Trajectory
                sampleTime += Runner.DeltaTime * speed;
                NewPosition = curve.Evaluate(sampleTime);
                NewRotation = curve.Evaluate(sampleTime + 0.001f) - transform.position;

                // Update Player
                if (NewRotation != Vector3.zero)
                {
                    transform.position = NewPosition;
                    transform.forward = NewRotation;
                }
                if (sampleTime >= 1)
                {
                    // Reset Player rotation
                    transform.rotation = Quaternion.Euler(0f, 90f, 0f);
                    transform.position = NewPosition;
                    IsJumping = false;

                    // Check to see if player got 1st place or died
                    if (score == 500)
                    {
                        TakeMoney();
                    }
                    if (score == 0)
                    {
                        TakeMoney();
                    }
                }
            }
        }

        private void Jump()
        {
            if (Object.HasStateAuthority)
            {
                // Generate a random number to where you jump
                /*  jumpPosition = UnityEngine.Random.Range(jumpPosition, 7);*/
                IsJumping = true;
                jumpPosition += 1;
                score = QuadraticCurveManager.Instance.MoveCurve(jumpPosition);
                sampleTime = 0f;
            }
        }

        private void TakeMoney()
        {
            if (Object.HasStateAuthority)
            {
                gameUIListener.AddScore(score);
                Level1Manager.Instance.PlayerTurn = false;
            }
        }
    }
}