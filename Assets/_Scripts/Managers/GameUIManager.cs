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
        [SerializeField] private GameObject chatBoxPanel;
        [SerializeField] private GameObject loadingPanel;

        public Dictionary<PlayerRef, PlayerData> PlayerDictionary;

        // Level1
        private const string tipsLvl1 = "Press Z to Jump \nPress X to CashOut";
        private const string tipsLvl2 = "Press Z to go Faster";
        private const string tipsLvl3 = "Use Your Mouse to doge the Projectiles";
        private const string tipsLvl4 = "Don't get crushed by the falling ceiling";

        //Singleton
        public static GameUIManager Instance;

        private void Awake()
        {
            Instance = this;
            // Set Panel for Game Tip
            DontDestroyOnLoad(this);
            PlayerDictionary = new();
        }

        public void UpdateLevelState(int _level)
        {
            switch (_level)
            {
                case 0:
                    MainMenu();
                    break;
                case 1:
                    Level1();
                    break;
                case 2:
                    Level2();
                    break;
                case 3:
                    Level3();
                    break;
                case 4:
                    Level4();
                    break;
            }
        }

        public void EnableChat(bool _var)
        {
            chatBoxPanel.SetActive(true);
        }

        private void MainMenu()
        {
            playerHp.SetActive(false);
            gameTipPanel.SetActive(false);
            scoreboardPanel.SetActive(false);
            chatBoxPanel.SetActive(true);
            loadingPanel.SetActive(false);
        }

        private void Level1()
        {
            gameTipPanel.SetActive(true);
            scoreboardPanel.SetActive(true);
            chatBoxPanel.SetActive(true);
            gameTipPanel.GetComponent<TMP_Text>().text = tipsLvl1;

            StartCoroutine(TipPanel(5f));
        }

        private void Level2()
        {
            gameTipPanel.GetComponent<TMP_Text>().text = tipsLvl2;
            StartCoroutine(TipPanel(5f));
        }

        private void Level3()
        {
            playerHp.SetActive(true);
            gameTipPanel.GetComponent<TMP_Text>().text = tipsLvl3;
            StartCoroutine(TipPanel(5f));
        }

        private void Level4()
        {
            playerHp.SetActive(false);
            gameTipPanel.GetComponent<TMP_Text>().text = tipsLvl4;
            StartCoroutine(TipPanel(5f));
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
        }

        public void UpdateUserName(PlayerRef _player, string _username)
        {
            PlayerDictionary[_player].UserName = _username;
            UpdateUI(_player);
        }

        public void UpdateScore(PlayerRef _player, int _score)
        {
            PlayerDictionary[_player].Score = _score;
            UpdateUI(_player);
        }

        public void UpdateHp(PlayerRef _player, int _hp)
        {
            PlayerDictionary[_player].Hp = _hp;
            UpdateUI(_player);
        }

        public void UpdateUI(PlayerRef _player)
        {
            Dictionary<PlayerRef, PlayerData> sortedList = PlayerDictionary.OrderBy(pair => pair.Value.Score).ToDictionary(pair => pair.Key, pair => pair.Value);

            PlayerData playerData = sortedList[_player];

            foreach (var key in sortedList)
            {
                PlayerData playerDataScore = key.Value;
                string playerScore = playerDataScore.UserName + " : " + playerDataScore.Score;
                playerDataScore.PlayerScore.GetComponent<TMP_Text>().text = playerScore;
            }

            /* // Update Score
             string playerScore = PlayerData.UserName + " : " + PlayerData.Score;
         PlayerData.PlayerScore.GetComponent<TMP_Text>().text = playerScore;*/

            string playerHp = playerData.UserName + "\n " + playerData.Hp + " / 100";
            playerData.PlayerHp.GetComponent<TMP_Text>().text = playerHp;
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