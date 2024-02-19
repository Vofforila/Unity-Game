using Data;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TryhardParty
{
    public class Level4PlayerScript : NetworkBehaviour
    {
        [Header("Scriptable")]
        public LocalData localData;
        public Firestore firestore;

        [Header("Event")]
        public UnityEvent updateScoreBoardEvent;
        public UnityEvent destroyPlayerEvent;

        private Vector3 clickPosition;

        private UnityEngine.AI.NavMeshAgent agent;

        private int finishPlace;
        private int score;

        /*
        private void Start()
        {
            if (localData.currentLvl == 4)
            {
                agent = gameObject.AddComponent<UnityEngine.AI.NavMeshAgent>();
                agent.speed = 8;
                agent.angularSpeed = 180f;
                agent.acceleration = 15f;
                agent.stoppingDistance = 0.1f;
                agent.radius = 0.5f;

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

        public override void Render()
        {
            MovePlayer();
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
                        // updateScoreBoardEvent.Invoke();
                        Debug.Log("DestoryPlayer - Event");
                        destroyPlayerEvent.Invoke();
                    }
                }
            }
            if (other.CompareTag("Coin"))
            {
                for (int i = 0; i < localData.playerListData.Count; i++)
                {
                    PlayerData playerData = localData.playerListData[i];
                    if (playerData.Username == firestore.accountFirebase.User)
                    {
                        score = 50;
                        //firestore.UpdateLobbyData(score, false, firestore.accountFirebase.User);
                        Debug.Log("Update Scoreboard - Event");
                        // updateScoreBoardEvent.Invoke();
                    }
                }
            }
        }

        public void MovePlayer()
        {
            if (clickPosition != new Vector3(0, 0, 0))
            {
                agent.SetDestination(new Vector3(clickPosition.x, 1.1f, clickPosition.z));
            }
        }
        */
    }
}