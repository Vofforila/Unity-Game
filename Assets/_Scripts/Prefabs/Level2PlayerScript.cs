using Data;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Singleton;
using TryhardParty;

namespace Player
{
    public class Level2PlayerScript : NetworkBehaviour
    {
        [Header("Scriptable")]
        [SerializeField] private LocalData localData;
        [SerializeField] private Firestore firestore;

        [Header("Internal")]
        [SerializeField] internal PlayerVisuals playerVisuals;

        [Networked] private TickTimer KeyCooldown { get; set; }
        [Networked] private float HorseSpeed { get; set; }
        [Networked] public NetworkButtons ButtonsPrevious { get; set; }

        // This is the backing value for our virtual bool.
        // Read Only
        [SerializeField] private Vector3 size = new(12, 12, 12);
        private int score;
        private int finishPlace;

        private void Awake()
        {
            if (localData.currentLvl != 2)
            {
                this.enabled = false;
            }
        }

        public void Init()
        {
            finishPlace = 4;
            HorseSpeed = 1f;
            KeyCooldown = TickTimer.CreateFromSeconds(Runner, 0.2f);
            EnablePlayer(true);
        }

        public override void Spawned()
        {
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
        }

        public void EnablePlayer(bool _var)
        {
            // Enable Visuals
            playerVisuals.SetVisuals(_var);
            playerVisuals.SetSize(size);
        }

        public void MakeHorseRun()
        {
            transform.position += HorseSpeed * Runner.DeltaTime * transform.forward;
        }

        // Check Collision
        public void OnTriggerEnter(Collider other)
        {
            // Make a 3d Rigid Body Component
            // Make diffrent scenarios for it
            // Check Collision
            // Make Score

            // Update score
            if (other.gameObject.CompareTag("Finish"))
            {
                Debug.Log("Collision");
                /*  if (finishPlace == 4)
                      score = 500;
                  if (finishPlace == 3)
                      score = 250;
                  if (finishPlace == 2)
                      score = 150;
                  if (finishPlace == 1)
                      score = 0;
                  finishPlace--;
  */
            }

            //firestore.UpdateScore(Runner.SessionInfo.Name, score);
        }
    }
}