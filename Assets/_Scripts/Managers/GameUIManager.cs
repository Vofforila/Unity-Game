using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Fusion;
using Data;
using System.Linq;

namespace UI
{
    public class GameUIManager : MonoBehaviour
    {
        [Header("Scriptable")]
        [SerializeField] private LocalData localData;

        [Header("HP")]
        [SerializeField] private GameObject playerHp;

        [Header("Scoreboard")]
        [SerializeField] private GameObject gameTipPanel;
        [SerializeField] private GameObject scoreboardPanel;
        [SerializeField] private GameObject playerScorePrefab;

        public Dictionary<PlayerRef, PlayerData> PlayerDictionary;

        //Singleton
        public static GameUIManager Instance;

        private void Awake()
        {
            Instance = this;
            // Set Panel for Game Tip
            DontDestroyOnLoad(this);
            PlayerDictionary = new();
        }

        private void OnEnable()
        {
            StartCoroutine(TipPanel(10f));
        }

        public IEnumerator TipPanel(float _time)
        {
            yield return new WaitForSecondsRealtime(_time);
            gameTipPanel.SetActive(false);
        }

        public void CreateUI(PlayerRef _player)
        {
            // Make a new Player UI Object
            PlayerData playerData = new();

            // Create the UI
            GameObject playerScore = Instantiate(playerScorePrefab, scoreboardPanel.transform);

            // Add the PlayerUI to PlayerData
            playerData.PlayerScore = playerScore;
            playerData.PlayerHp = playerHp;

            // Attach the PlayerData to a List
            PlayerDictionary.Add(_player, playerData);

            // Update UI
            UpdateUI(_player);

            Debug.Log(_player + " - Create UI : " + PlayerDictionary[_player].UserName);
        }

        public void UpdateUserName(PlayerRef _player, string _username)
        {
            PlayerDictionary[_player].UserName = _username;
            UpdateUI(_player);

            Debug.Log(_player + " - Username Update : " + PlayerDictionary[_player].UserName);
        }

        public void UpdateScore(PlayerRef _player, int _score)
        {
            PlayerDictionary[_player].Score = _score;
            UpdateUI(_player);
            Debug.Log(_player + " - Score Update : " + PlayerDictionary[_player].Score);
        }

        public void UpdateHp(PlayerRef _player, int _hp)
        {
            PlayerDictionary[_player].Hp = _hp;
            UpdateUI(_player);
            Debug.Log(_player + " -Hp Update : " + PlayerDictionary[_player].Hp);
        }

        public void UpdateUI(PlayerRef _player)
        {
            /* // Get the PlayerData
             PlayerData PlayerData = PlayerDictionary[_player];*/

            Dictionary<PlayerRef, PlayerData> sortedList = PlayerDictionary.OrderBy(pair => pair.Value.Score).ToDictionary(pair => pair.Key, pair => pair.Value);

            PlayerData PlayerData = sortedList[_player];

            // Update Score
            string playerScore = PlayerData.UserName + " : " + PlayerData.Score;
            PlayerData.PlayerScore.GetComponent<TMP_Text>().text = playerScore;

            string playerHp = PlayerData.UserName + "\n " + PlayerData.Hp + " / 100";
            PlayerData.PlayerHp.GetComponent<TMP_Text>().text = playerHp;
        }

        public class PlayerData
        {
            public string UserName { get; set; }
            public int Score { get; set; }
            public int Hp { get; set; }
            public GameObject PlayerScore { get; set; }
            public GameObject PlayerHp { get; set; }

            public PlayerData()
            {
                UserName = "";
                Score = 0;
                Hp = 100;
                PlayerScore = null;
                PlayerHp = null;
            }
        }
    }
}