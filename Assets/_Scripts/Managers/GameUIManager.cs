using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Fusion;
using Data;
using System.Linq;
using Database;

namespace UI
{
    public class GameUIManager : MonoBehaviour
    {
        [Header("Scriptable")]
        [SerializeField] private LocalData localData;
        [SerializeField] private Firestore firebase;

        [Header("HP")]
        [SerializeField] private GameObject playerHp;

        [Header("Tip Panel")]
        [SerializeField] private GameObject gameTipPanel;

        [Header("Scoreboard")]
        [SerializeField] private GameObject scoreboardPanel;
        [SerializeField] private GameObject playerScorePrefab;

        [Header("Chat")]
        [SerializeField] private GameObject chatBoxPanel;

        [Header("Loading Screen")]
        [SerializeField] private GameObject loadingPanel;

        public Dictionary<PlayerRef, PlayerData> PlayerDictionary;

        // Level1
        private const string tipsLvl1 = "Press Z to Jump \nPress X to CashOut";
        private const string tipsLvl2 = "Press Z to go Faster";
        private const string tipsLvl3 = "Use Your Mouse to doge the Projectiles";
        private const string tipsLvl4 = "Don't get crushed by the falling ceiling";

        private List<GameObject> scoreList;

        private LocalPlayerData localPlayerData;

        //Singleton
        public static GameUIManager Instance;

        private void Awake()
        {
            Instance = this;
            // Set Panel for Game Tip
            DontDestroyOnLoad(this);
            PlayerDictionary = new();
            scoreList = new();
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
            scoreList.Add(playerScore);

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

        public void UpdateUI(PlayerRef _player)
        {
            Dictionary<PlayerRef, PlayerData> sortedList = PlayerDictionary.OrderByDescending(pair => pair.Value.Score).ToDictionary(pair => pair.Key, pair => pair.Value);

            PlayerData playerData = sortedList[_player];

            int x = 0;
            foreach (var key in sortedList)
            {
                PlayerData playerDataScore = key.Value;
                string playerScore = playerDataScore.UserName + " : " + playerDataScore.Score;
                scoreList[x++].GetComponent<TMP_Text>().text = playerScore;
            }
        }

        public void CreateLocalUI()
        {
            localPlayerData = new();
            localPlayerData.UserName = firebase.accountFirebase.User;
            localPlayerData.PlayerHp = playerHp;
            UpdateLocalUI();
        }

        public void UpdateHp(int _hp)
        {
            localPlayerData.Hp = _hp;
            UpdateLocalUI();
        }

        public void UpdateLocalUI()
        {
            string playerHp = localPlayerData.UserName + "\n " + localPlayerData.Hp + " / 100";
            localPlayerData.PlayerHp.GetComponent<TMP_Text>().text = playerHp;
        }

        public class PlayerData
        {
            public string UserName { get; set; }
            public int Score { get; set; }

            public PlayerData()
            {
                UserName = "";
                Score = 0;
            }
        }

        public class LocalPlayerData
        {
            public string UserName { get; set; }
            public int Hp { get; set; }
            public GameObject PlayerHp { get; set; }

            public LocalPlayerData()
            {
                UserName = "";
                Hp = 20;
            }
        }
    }
}