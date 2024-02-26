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
<<<<<<< Updated upstream

        public void Init()
        {
=======
        [SerializeField] private Vector3 size = new(0.8f, 0.8f, 0.8f);
        [SerializeField] private bool[] constrains = { false, false, false, false, false, false };
        [SerializeField] private float mass = 10f;
        public float MaxHitDistance = 1000f;

        public LayerMask targetLayer;

        [Header("Agent Settings")]
        private float offset = 1.5f;
        private NavMeshSurface navMesh;
        private NavMeshAgent agent;

        private void Awake()
        {
            if (localData.currentLvl != 3)
            {
                enabled = false;
            }
            playerCamera = GameObject.Find("CameraPrefab(Clone)").transform;
            targetLayer = LayerMask.GetMask("HitZone");

            /*    Destroy(gameObject.GetComponent<NetworkRigidbody3D>());
                Destroy(gameObject.GetComponent<Rigidbody>());*/
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
                agent = gameObject.GetComponent<NavMeshAgent>();

                /*  EnablePlayer(true);*/
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
                        ClickPosition = hit.point;
                    }

                    /*  MovePlayer(inputData.clickPosition);*/
                    KeyCooldown = TickTimer.CreateFromSeconds(Runner, 0.2f);
                }
            }

            MovePlayer(ClickPosition);
        }

        public void EnablePlayer(bool _var)
        {
            // Enable Visuals

            playerVisuals.SetVisuals(_var);
            playerVisuals.SetSize(size);
            /* playerVisuals.SetRigidbody(true, constrains, mass);*/
        }

        public void MovePlayer(Vector3 _clickPosition)
        {
            agent.SetDestination(_clickPosition);
>>>>>>> Stashed changes
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