using Data;
using Database;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        private const string tipsLvl1 = "Press Z to Jump \nPress X to CashOut";
        private const string tipsLvl2 = "Press Z to go Faster";
        private const string tipsLvl3 = "Use Your Mouse to doge the Projectiles";
        private const string tipsLvl4 = "Don't get crushed by the falling ceiling";

        [Header("Scoreboard")]
        [SerializeField] private GameObject scoreboardPanel;
        [SerializeField] private GameObject playerScorePrefab;

        [Header("Chat")]
        [SerializeField] private GameObject chatBoxPanel;

        [Header("Lobby")]
        [SerializeField] private GameObject bannerPrefab;

        [Header("Loading Screen")]
        [SerializeField] private GameObject loadingPanel;

        [Header("Banner Sprites")]
        [SerializeField] public List<Sprite> bannerSprites;

        public Dictionary<PlayerRef, PlayerData> playerDictionary;

        private LocalPlayerData localPlayerData;
        private UIComponentsData componentsData;
        private Transform BannerLayoutTransform;

        public static GameUIManager Instance;

        #region Awake

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(this);
            playerDictionary = new();
            componentsData = new();
        }

        #endregion Awake

        #region Start

        private void Start()
        {
            BannerLayoutTransform = UIManager.Instance.bannerLayout;
        }

        #endregion Start

        #region GameUI State

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
                case -1:
                    MainMenu();
                    break;
            }
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
            playerHp.SetActive(false);

            gameTipPanel.GetComponent<TMP_Text>().text = tipsLvl1;
            StartCoroutine(TipPanel(5f));
            UpdateUI();
        }

        private void Level2()
        {
            gameTipPanel.SetActive(true);
            scoreboardPanel.SetActive(true);
            chatBoxPanel.SetActive(true);
            playerHp.SetActive(false);

            gameTipPanel.GetComponent<TMP_Text>().text = tipsLvl2;
            StartCoroutine(TipPanel(5f));
        }

        private void Level3()
        {
            gameTipPanel.SetActive(true);
            scoreboardPanel.SetActive(true);
            chatBoxPanel.SetActive(true);
            playerHp.SetActive(true);

            gameTipPanel.GetComponent<TMP_Text>().text = tipsLvl3;
            StartCoroutine(TipPanel(5f));
        }

        private void Level4()
        {
            gameTipPanel.SetActive(true);
            scoreboardPanel.SetActive(true);
            chatBoxPanel.SetActive(true);
            playerHp.SetActive(false);

            gameTipPanel.GetComponent<TMP_Text>().text = tipsLvl4;
            StartCoroutine(TipPanel(5f));
        }

        public IEnumerator TipPanel(float _time)
        {
            yield return new WaitForSecondsRealtime(_time);
            gameTipPanel.SetActive(false);
        }

        #endregion GameUI State

        #region GameUiListener Functions

        // Create new UI
        public void CreateUI(PlayerRef _player)
        {
            PlayerData playerData = new();

            GameObject banner = Instantiate(bannerPrefab, BannerLayoutTransform);
            GameObject playerScore = Instantiate(playerScorePrefab, scoreboardPanel.transform);

            componentsData.ScoreList.Add(playerScore);
            componentsData.BannerList.Add(banner);

            // Add banner
            playerDictionary.Add(_player, playerData);

            // Update UI
            UpdateUI();
        }

        public void UpdateUserName(PlayerRef _player, string _username)
        {
            playerDictionary[_player].UserName = _username;
            UpdateUI();
        }

        public void UpdateRank(PlayerRef _player, string _rank)
        {
            playerDictionary[_player].Rank = _rank;
            UpdateUI();
        }

        public void UpdateScore(PlayerRef _player, int _score)
        {
            Debug.Log(playerDictionary[_player].Score + _score);
            playerDictionary[_player].Score = _score;
            UpdateUI();
        }

        public void UpdateUI()
        {
            // Sort Score Ascending
            Dictionary<PlayerRef, PlayerData> sortedList = playerDictionary.
                OrderByDescending(pair => pair.Value.Score).
                ToDictionary(pair => pair.Key, pair => pair.Value);

            int x = 0;
            foreach (var key in sortedList)
            {
                PlayerData playerDataScore = key.Value;

                string playerScore = playerDataScore.UserName + " : " + playerDataScore.Score;
                componentsData.ScoreList[x].GetComponent<TMP_Text>().text = playerScore;

                // Update Lobby Data
                if (localData.currentLvl == 0)
                {
                    componentsData.BannerList[x].GetComponentInChildren<TMP_Text>().text = playerDataScore.UserName;
                    Image bannerImage = componentsData.BannerList[x].GetComponentInChildren<Image>();
                    switch (playerDataScore.Rank)
                    {
                        case "Iron":
                            bannerImage.sprite = bannerSprites[0];
                            break;
                        case "Bronze":
                            bannerImage.sprite = bannerSprites[1];
                            break;
                        case "Silver":
                            bannerImage.sprite = bannerSprites[2];
                            break;
                        case "Gold":
                            bannerImage.sprite = bannerSprites[3];
                            break;
                        case "Platinum":
                            bannerImage.sprite = bannerSprites[4];
                            break;
                        case "Diamond":
                            bannerImage.sprite = bannerSprites[5];
                            break;
                        case "Master":
                            bannerImage.sprite = bannerSprites[6];
                            break;
                        case "GrandMaster":
                            bannerImage.sprite = bannerSprites[7];
                            break;
                        case "Challanger":
                            bannerImage.sprite = bannerSprites[8];
                            break;
                    }
                }
                x++;
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
    }

    #endregion GameUiListener Functions

    public class UIComponentsData
    {
        public List<GameObject> ScoreList { get; set; }
        public List<GameObject> BannerList { get; set; }

        public UIComponentsData()
        {
            ScoreList = new();
            BannerList = new();
        }
    }

    public class PlayerData
    {
        public string UserName { get; set; }
        public string Rank { get; set; }
        public int Score { get; set; }

        public PlayerData()
        {
            UserName = "";
            Rank = "Bronze";
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