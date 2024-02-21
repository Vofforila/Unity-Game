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
        public LocalData localData;

        public Firestore firestore;

        [Header("Event")]
        public UnityEvent updateScoreBoardEvent;

        public UnityEvent destroyPlayerEvent;

        private Vector3 clickPosition;

        private NavMeshAgent agent;

        private int finishPlace;
        private int score;

        public void Init()
        {
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