using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;
using Data;
using TryhardParty;

namespace Player
{
    public class Level3PlayerScript : NetworkBehaviour
    {
        [Header("Scriptable")]
        [SerializeField] private LocalData localData;
        [SerializeField] private Firestore firestore;

        [Header("Internal")]
        [SerializeField] internal PlayerVisuals playerVisuals;

        [Header("Camera")]
        [SerializeField] private Transform playerCamera;

        [Networked] private Vector3 ClickPosition { get; set; }
        [Networked] public NetworkButtons ButtonsPrevious { get; set; }
        [Networked] private TickTimer KeyCooldown { get; set; }

        private NavMeshAgent agent;

        private int finishPlace;
        private int score;
        [SerializeField] private Vector3 size = new(0.8f, 0.8f, 0.8f);
        [SerializeField] private bool[] constrains = { false, false, false, false, true, false };
        [SerializeField] private float mass = 10f;
        public float MaxHitDistance = 1000f;

        public LayerMask hitZone;

        private void Awake()
        {
            if (localData.currentLvl != 3)
            {
                enabled = false;
            }
            playerCamera = GameObject.Find("CameraPrefab(Clone)").transform;
            hitZone = LayerMask.GetMask("HitZone");
        }

        public void Init()
        {
            if (localData.currentLvl == 3)
            {
                if (Object.HasInputAuthority)
                {
                    agent = gameObject.AddComponent<NavMeshAgent>();
                    KeyCooldown = TickTimer.CreateFromSeconds(Runner, 0.2f);
                }
            }
        }

        public override void Spawned()
        {
            if (localData.currentLvl == 3)
            {
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
                    Debug.Log("Press" + playerCamera.position + inputData.clickPosition);
                    var hitOptions = HitOptions.IncludePhysX;

                    if (Runner.LagCompensation.Raycast(playerCamera.position, inputData.clickPosition, MaxHitDistance,
                        Object.InputAuthority, out var hit, hitZone, hitOptions))
                    {
                        Debug.Log("Move");
                        MovePlayer(hit.Point);
                    }
                    KeyCooldown = TickTimer.CreateFromSeconds(Runner, 0.2f);
                }
            }
        }

        public void EnablePlayer(bool _var)
        {
            // Enable Visuals
            playerVisuals.SetVisuals(_var);
            playerVisuals.SetSize(size);
            playerVisuals.SetRigidbody(false, constrains, mass);
        }

        public void MovePlayer(Vector3 _clickPosition)
        {
            agent.SetDestination(_clickPosition);
        }

        /*
        private void Start()
        {
            if (localData.currentLvl == 3)
            {
                agent = gameObject.AddComponent<NavMeshAgent>();

                clickPosition = gameObject.transform.position;
                finishPlace = 4;
            }
        }

        public override void FixedUpdateNetwork()
        {
            if (GetInput(out NetworkInputData data))
            {
                clickPosition = data.clickPosition;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("Collision");
            if (other.CompareTag("Projectile"))
            {
                for (int i = 0; i < localData.playerListData.Count; i++)
                {
                    PlayerData playerData = localData.playerListData[i];
                    if (playerData.Username == firestore.accountFirebase.User)
                    {
                        if (finishPlace == 4)
                            score = 150;
                        if (finishPlace == 3)
                            score = 250;
                        if (finishPlace == 2)
                            score = 350;
                        if (finishPlace == 1)
                            score = 500;
                        finishPlace--;

                        //firestore.UpdateLobbyData(score, false, firestore.accountFirebase.User);
                        //  localData.playerListData[i].Score += score;

                        Debug.Log("Update Scoreboard - Event");
                        updateScoreBoardEvent.Invoke();
                        Debug.Log("DestoryPlayer - Event");
                        destroyPlayerEvent.Invoke();
                    }
                }
            }
        }

        public override void Render()
        {
            MovePlayer();
        }

        public void MovePlayer()
        {
            if (clickPosition != new Vector3(0, 0, 0))
                agent.SetDestination(clickPosition);
        }
         */
    }
}