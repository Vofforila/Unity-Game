using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;
using Data;
using TryhardParty;
using Unity.AI.Navigation;
using Fusion.Addons.Physics;

namespace Player
{
    public class Level3PlayerScript : NetworkBehaviour
    {
        [Header("Scriptable")]
        public LocalData localData;

        public Firestore firestore;

        [Header("Event")]
        public UnityEvent updateScoreBoardEvent;

        public UnityEvent destroyPlayerEvent;

        private Vector3 clickPosition;

        private int finishPlace;
        private int score;

        [SerializeField] private Vector3 size = new(0.8f, 0.8f, 0.8f);
        [SerializeField] private bool[] constrains = { false, false, false, false, false, false };
        [SerializeField] private float mass = 10f;
        public float MaxHitDistance = 1000f;

        public LayerMask targetLayer;

        [Header("Agent Settings")]
        private float offset = 1.5f;
        private NavMeshSurface navMesh;
        private NavMeshAgent agent;

        [Networked] private TickTimer KeyCooldown { get; set; }
        [Networked] public NetworkButtons ButtonsPrevious { get; set; }

        internal PlayerVisuals playerVisuals;

        private Transform playerCamera;

        public void Init()
        {
            if (localData.currentLvl == 3)
            {
                gameObject.AddComponent<NetworkTransform>();
                KeyCooldown = TickTimer.CreateFromSeconds(Runner, 0.2f);
            }
        }

        public override void Spawned()
        {
            if (localData.currentLvl == 3)
            {
                agent = gameObject.GetComponent<NavMeshAgent>();

                EnablePlayer(true);
            }
        }

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

                if (pressed.IsSet(GameButton.LeftClick) && KeyCooldown.Expired(Runner) == true)
                {
                    /* Debug.Log("Move");
                     Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                     if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, targetLayer))
                     {
                         ClickPosition = hit.point;
                     }

                     MovePlayer(inputData.clickPosition);
                     KeyCooldown = TickTimer.CreateFromSeconds(Runner, 0.2f);*/
                }
            }

            /*MovePlayer(ClickPosition);*/
        }

        public void EnablePlayer(bool _var)
        {
            // Enable Visuals

            playerVisuals.SetVisuals(_var);
            playerVisuals.SetSize(size);
            playerVisuals.SetRigidbody(true, constrains, mass);
        }

        public void MovePlayer(Vector3 _clickPosition)
        {
            agent.SetDestination(_clickPosition);
        }
    }
}