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
    public class Level3PlayerScript : NetworkBehaviour
    {
        [Header("Scriptable")]
        [SerializeField] private LocalData localData;
        [SerializeField] private Firestore firestore;

        [Header("Internal")]
        [SerializeField] internal PlayerVisuals playerVisuals;
        [SerializeField] internal GameUIListener gameUIListener;

        [Header("ReadOnly")]
        [Header("Object")]
        [SerializeField] private float size = 1f;
        [SerializeField] private bool[] constrains = null;
        [SerializeField] private bool isKinematic = true;
        [SerializeField] private float mass = 0f;

        [Header("Agent Settings")]
        private NavMeshAgent agent;
        private NavMeshSurface surface;
        [SerializeField] private float agentoffset = 1.5f;
        [SerializeField] private float speed = 15f;
        [SerializeField] private float angularSpeed = 129f;
        [SerializeField] private float acceleration = 8f;
        [SerializeField] private float stoppingDistance = 0f;
        [SerializeField] private float obstacleRadius = 0.5f;
        [SerializeField] private float obstacleHeight = 2f;

        [Networked] private int PlayerHp { get; set; }
        [Networked] public NetworkButtons ButtonsPrevious { get; set; }

        private ChangeDetector changeDetector;

        [Header("Game")]
        private int score;
        private bool isAlive;

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
                PlayerHp = 20;
            }
        }

        public override void Spawned()
        {
            if (localData.currentLvl == 3)
            {
                agent = gameObject.AddComponent<NavMeshAgent>();
                surface = GameObject.Find("NavMeshManager").GetComponent<NavMeshSurface>();
                agent.agentTypeID = surface.agentTypeID;

                playerVisuals.SetAgent(agent, agentoffset, speed, angularSpeed, acceleration, stoppingDistance, obstacleRadius, obstacleHeight);
                playerVisuals.SetPlayer(_visuals: true, _size: size, _isKinematic: isKinematic, _constrains: constrains, _mass: mass);

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
                    case nameof(PlayerHp):
                        if (PlayerHp <= 0)
                        {
                            Debug.Log("Dead");
                            SoundManager.Instance.PlaySound("dmg");
                            if (Object.HasInputAuthority)
                            {
                                isAlive = false;
                                RPC_PlayerDead();
                            }
                            playerVisuals.SetVisuals(false);
                        }
                        else if (Object.HasInputAuthority)
                        {
                            SoundManager.Instance.PlaySound("dmg");
                            gameUIListener.UpdateHp(PlayerHp);
                        }
                        break;
                }
            }
        }

        public void OnTriggerEnter(Collider other)
        {
            if (Object.HasStateAuthority && other.gameObject.CompareTag("Bullet") && localData.currentLvl == 3 && isAlive == true)
            {
                // take dmg
                if (PlayerHp > 0)
                {
                    PlayerHp -= 10;
                    if (PlayerHp < 0)
                    {
                        PlayerHp = 0;
                    }
                }
            }
        }

        public void MovePlayer(Vector3 _clickPosition)
        {
            if (Object.HasStateAuthority)
            {
                agent.SetDestination(_clickPosition);
            }
        }

        [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
        private void RPC_PlayerDead()
        {
            if (Level3Manager.Instance.FinishPlace == 4)
            {
                score = 250;
                gameUIListener.AddScore(score);
            }
            if (Level3Manager.Instance.FinishPlace == 3)
            {
                score = 350;
                gameUIListener.AddScore(score);
            }
            if (Level3Manager.Instance.FinishPlace == 2)
            {
                score = 400;
                gameUIListener.AddScore(score);
            }
            if (Level3Manager.Instance.FinishPlace == 1)
            {
                score = 500;
                gameUIListener.AddScore(score);
            }

            Level3Manager.Instance.FinishPlace--;
        }
    }
}