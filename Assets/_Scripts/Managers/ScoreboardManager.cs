using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Data;

namespace TryhardParty
{
    public class ScoreboardManager : MonoBehaviour
    {
        [Header("Scriptable")]
        [SerializeField] private LocalData localData;
        [SerializeField] private Firestore firestore;

        [SerializeField]
        private GameObject playerScorePrefab;

        private GameObject instanciatedPrefab;
        private List<TMP_Text> scoreList;

        private void Awake()
        {
            scoreList = new();
            for (int i = 0; i < firestore.lobbydata.HostPlayerList.Count; i++)
            {
                instanciatedPrefab = Instantiate(playerScorePrefab, this.transform);
                TMP_Text scoreText = instanciatedPrefab.GetComponent<TMP_Text>();
                scoreList.Add(scoreText);
            }
            UpdateLobbyDataEvent();
        }

        public void UpdateLobbyDataEvent()
        {
            Debug.Log("Callback");
            for (int i = 0; i < firestore.lobbydata.HostPlayerList.Count; i++)
            {
                string username = firestore.lobbydata.HostPlayerList[i].ToString();
                string score = firestore.lobbydata.ScoreList[username].ToString();

                scoreList[i].text = username + " : " + score;
            }
        }
    }
}