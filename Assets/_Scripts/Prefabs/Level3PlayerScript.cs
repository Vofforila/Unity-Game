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

        private int finishPlace;
        private int score;

        [SerializeField] private Vector3 size = new(0.8f, 0.8f, 0.8f);
        [SerializeField] private bool[] constrains = { false, false, false, false, false, false };
        [SerializeField] private float mass = 10f;
        public float MaxHitDistance = 1000f;

        public LayerMask targetLayer;

        [Header("Agent Settings")]
        private NavMeshAgent agent;

        [Networked] private TickTimer KeyCooldown { get; set; }
        [Networked] public NetworkButtons ButtonsPrevious { get; set; }

        internal PlayerVisuals playerVisuals;

        public void Awake()
        {
            if (localData.currentLvl != 3)
            {
                enabled = false;
            }
        }

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
                agent = gameObject.AddComponent<NavMeshAgent>();
                playerVisuals.SetPlayer(_visuals: true, _size: size, _isKinematic: false, _constrains: constrains, _mass: mass);
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
                    Debug.Log("Move");
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, targetLayer))
                    {
                        if (Object.HasStateAuthority)
                        {
                            MovePlayer(hit.point);
                        }
                    }
                    KeyCooldown = TickTimer.CreateFromSeconds(Runner, 0.2f);
                }
            }
        }

        public void MovePlayer(Vector3 _clickPosition)
        {
            agent.SetDestination(_clickPosition);
        }
    }
}