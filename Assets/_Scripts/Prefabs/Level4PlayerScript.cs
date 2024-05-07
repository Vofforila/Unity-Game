using Data;
using Database;
using Fusion;
using Host;
using PlayerInput;
using UI;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

namespace Player
{
    public class Level4PlayerScript : NetworkBehaviour
    {
        [Header("Scriptable")]
        [SerializeField] private LocalData localData;
        [SerializeField] private Firestore firestore;

        [Header("Internal")]
        [SerializeField] internal PlayerVisuals playerVisuals;
        [SerializeField] internal GameUIListener gameUIListener;

        [Header("ReadOnly")]
        [Header("Object")]
        [SerializeField] private float size = 0.2f;
        [SerializeField] private bool[] constrains = null;
        [SerializeField] private bool isKinematic = true;
        [SerializeField] private float mass = 0f;

        [Header("Agent Settings")]
        private NavMeshAgent agent;
        private NavMeshSurface surface;
        [SerializeField] private float agentoffset = 1.5f;
        [SerializeField] private float speed = 15f;
        [SerializeField] private float angularSpeed = 180f;
        [SerializeField] private float acceleration = 15f;
        [SerializeField] private float stoppingDistance = 0.1f;
        [SerializeField] private float obstacleRadius = 0.5f;
        [SerializeField] private float obstacleHeight = 2f;

        [Networked] private PlayerRef Player { get; set; }
        [Networked] public NetworkButtons ButtonsPrevious { get; set; }

        private ChangeDetector changeDetector;

        private int score;
        private bool isAlive;

        public void Awake()
        {
            if (localData.currentLvl != 4)
            {
                enabled = false;
            }
        }

        public void Init()
        {
            if (localData.currentLvl == 4)
            {
                Player = PlayerRef.None;
            }
        }

        public override void Spawned()
        {
            if (localData.currentLvl == 4)
            {
                agent = gameObject.AddComponent<NavMeshAgent>();
                surface = GameObject.Find("NavMeshManager").GetComponent<NavMeshSurface>();
                agent.agentTypeID = surface.agentTypeID;

                playerVisuals.SetPlayer(true, size, isKinematic, constrains, mass);
                playerVisuals.SetAgent(agent, agentoffset, speed, angularSpeed, acceleration, stoppingDistance, obstacleRadius, obstacleHeight);

                isAlive = true;
                changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
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

                if (inputData.raycast != Vector3.zero)
                {
                    MovePlayer(inputData.raycast);
                }
            }
        }

        public override void Render()
        {
            foreach (var change in changeDetector.DetectChanges(this, out var previousBuffer, out var currentBuffer))
            {
                switch (change)
                {
                    case nameof(Player):
                        if (Object.HasInputAuthority)
                        {
                            SoundManager.Instance.PlaySound("crush");
                            RPC_PlayerDead();
                        }
                        playerVisuals.SetVisuals(false);
                        isAlive = false;
                        break;
                }
            }
        }

        public void OnTriggerEnter(Collider other)
        {
            if (Object.HasStateAuthority && other.gameObject.CompareTag("Projectile") && localData.currentLvl == 4 && isAlive == true)
            {
                Debug.Log("Collision");
                Player = Object.InputAuthority;
            }
        }

        public void MovePlayer(Vector3 _clickPosition)
        {
            if (HasStateAuthority)
            {
                agent.SetDestination(_clickPosition);
            }
        }

        [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
        private void RPC_PlayerDead()
        {
            if (Level4Manager.Instance.FinishPlace == 4)
            {
                score = 250;
                gameUIListener.AddScore(score);
            }
            if (Level4Manager.Instance.FinishPlace == 3)
            {
                score = 350;
                gameUIListener.AddScore(score);
            }
            if (Level4Manager.Instance.FinishPlace == 2)
            {
                score = 400;
                gameUIListener.AddScore(score);
            }
            if (Level4Manager.Instance.FinishPlace == 1)
            {
                score = 500;
                gameUIListener.AddScore(score);
            }

            Level4Manager.Instance.FinishPlace--;
        }
    }
}