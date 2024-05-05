using Data;
using Database;
using Fusion;
using SpecialFunction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Database.Firestore;
using static UI.GameUIManager;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("LocalData")]
        [SerializeField] private Firestore firestore;
        [SerializeField] private LocalData localdata;
        [SerializeField] private SpecialFunctions specialFunctions;

        [Header("Prefabs")]
        [SerializeField] private GameObject friendPrefab;
        [SerializeField] private GameObject friendRequestPrefab;
        [SerializeField] private GameObject lobbyInvitePrefab;

        // Remove public on release
        [Header("Canvases")]
        [SerializeField] public GameObject authCanvas;
        [SerializeField] public GameObject mainMenuLoadingCanvas;
        [SerializeField] public GameObject mainMenuCanvas;
        [SerializeField] private GameObject popupCanvas;

        [Header("AuthPanels")]
        [SerializeField] private GameObject registerPanel;
        [SerializeField] private GameObject loginPanel;
        [SerializeField] private GameObject forgotPasswordPanel;

        [Header("Loading")]
        [SerializeField] private Slider loadingBar;
        [SerializeField] private TMP_Text loadingPercentage;

        [Header("ProfilePanel")]
        [SerializeField] private TMP_Text currentUserName;
        [SerializeField] private TMP_Text winrate;
        [SerializeField] private TMP_Text rankPoints;
        [SerializeField] private Image playerIcon;

        [Header("Add Freind Panel")]
        [SerializeField] private GameObject addFriendPanel;
        [SerializeField] private TMP_InputField addFriendInput;
        [SerializeField] private GameObject sentFriendRequestScrollViewContent;
        [SerializeField] private GameObject sentFriendRequestScrollView;
        [SerializeField] private GameObject noSentFriendRequest;
        [SerializeField] private GameObject sentFriendPrefab;

        [Header("Friend Request Panel")]
        [SerializeField] private GameObject friendRequestButtonPanel;
        [SerializeField] private GameObject friendRequestPanel;
        [SerializeField] private GameObject friendRequestScrollContent;
        [SerializeField] private TMP_Text friendRequestsNumber1;
        [SerializeField] private TMP_Text friendRequestsNumber2;

        [Header("Friend List")]
        [SerializeField] private GameObject friendScrollViewContent;
        [SerializeField] private GameObject noFriend;
        [SerializeField] private GameObject friendScrollView;
        [SerializeField] private GameObject friendHeaderPrefab;

        [Header("InviteToLobbyPanel")]
        [SerializeField] private GameObject inviteToLobbyPanel;
        [SerializeField] private GameObject inviteToLobbyScrollViewContent;

        [Header("Lobby")]
        [SerializeField] private GameObject playButton;
        [SerializeField] private GameObject startButton;
        [SerializeField] private GameObject lobbyPanel;
        [SerializeField] private GameObject homePanel;
        public Transform bannerLayout;

        [Header("Statistics")]
        [SerializeField] private GameObject matchStatisticsPanel;
        [SerializeField] private GameObject playersStatisticPanel;
        [SerializeField] private GameObject rankPointsGained;
        [SerializeField] private GameObject playerStatsPrefab;

        [Header("Choose Icon Panel")]
        [SerializeField] private GameObject chooseIconPanel;
        [SerializeField] private List<Sprite> playerIcons;

        [Header("Setting Panel")]
        [SerializeField] private GameObject settingCanvas;

        public static UIManager Instance;

        private bool loadingUITask = false;

        #region Awake & Start & OnQuit

        // Close Everything
        private void Awake()
        {
            Instance = this;

            authCanvas.SetActive(true);
            popupCanvas.SetActive(true);

            // Disable All Canvases
            mainMenuCanvas.SetActive(false);
            mainMenuLoadingCanvas.SetActive(false);

            // Disable All Pop-ups
            friendRequestPanel.SetActive(false);
            addFriendPanel.SetActive(false);

            OpenLoginPanel();
        }

        private void Start()
        {
            if (localdata.currentLvl > 0)
            {
                ShowStatisticPanelEvent();
                localdata.currentLvl = 0;
            }
        }

        private void OnApplicationQuitting()
        {
            firestore.StateChange(false);
        }

        #endregion Awake & Start & OnQuit

        #region Open/Close Canvases

        public void OpenLoginPanel()
        {
            loginPanel.SetActive(true);
            registerPanel.SetActive(false);
            forgotPasswordPanel.SetActive(false);
            mainMenuLoadingCanvas.SetActive(false);
            mainMenuCanvas.SetActive(false);
        }

        public void OpenRegisterPanel()
        {
            loginPanel.SetActive(false);
            registerPanel.SetActive(true);
            forgotPasswordPanel.SetActive(false);
            mainMenuLoadingCanvas.SetActive(false);
            mainMenuCanvas.SetActive(false);
        }

        public void OpenForgotPasswordPanel()
        {
            loginPanel.SetActive(false);
            registerPanel.SetActive(false);
            forgotPasswordPanel.SetActive(true);
            mainMenuLoadingCanvas.SetActive(false);
            mainMenuCanvas.SetActive(false);
        }

        public void OpenMainMenuLoadingCanvas()
        {
            loginPanel.SetActive(false);
            registerPanel.SetActive(false);
            forgotPasswordPanel.SetActive(false);
            mainMenuLoadingCanvas.SetActive(true);
            mainMenuCanvas.SetActive(false);
        }

        public void OpenMainMenuCanvas()
        {
            loginPanel.SetActive(false);
            registerPanel.SetActive(false);
            forgotPasswordPanel.SetActive(false);
            mainMenuLoadingCanvas.SetActive(false);
            mainMenuCanvas.SetActive(true);
        }

        #endregion Open/Close Canvases

        #region Pop-ups

        public void EnableFriendRequestPanel(bool _val)
        {
            friendRequestPanel.SetActive(_val);
        }

        public void EnableAddFriendPanel(bool _var)
        {
            addFriendPanel.SetActive(_var);
        }

        public void EnableChooseIcons(bool _var)
        {
            chooseIconPanel.SetActive(_var);
        }

        #endregion Pop-ups

        #region Statistic Panel

        public async void ShowStatisticPanelEvent()
        {
            Debug.Log("Callback");
            mainMenuCanvas.SetActive(true);
            homePanel.SetActive(false);
            matchStatisticsPanel.SetActive(true);

            Dictionary<PlayerRef, PlayerData> playerDictionary = GameUIManager.Instance.playerDictionary;

            Dictionary<PlayerRef, PlayerData> sortedList = playerDictionary.OrderByDescending(pair => pair.Value.Score).ToDictionary(pair => pair.Key, pair => pair.Value);

            int i;
            int place = 1;
            foreach (var key in sortedList)
            {
                i = 0;

                GameObject playerStatistics = Instantiate(playerStatsPrefab, playersStatisticPanel.transform);

                await firestore.GetPlayerIcon(key.Value.UserName);

                Debug.Log(firestore.playerIcon);

                playerStatistics.GetComponentInChildren<Image>().sprite = playerIcons[firestore.playerIcon];

                TMP_Text[] stats = playerStatistics.GetComponentsInChildren<TMP_Text>();

                stats[i++].text = (place++).ToString();
                stats[i++].text = key.Value.UserName;
                stats[i++].text = key.Value.Score.ToString();

                rankPointsGained.GetComponent<TMP_Text>().text = key.Value.Score.ToString();
            }
        }

        public void EnableStatisticsPanel(bool _var)
        {
            matchStatisticsPanel.SetActive(_var);
        }

        #endregion Statistic Panel

        #region Lobby Panel

        public void EnableHomePanel(bool _var)
        {
            homePanel.SetActive(_var);
        }

        public void EnableLobbyPanel(bool _var)
        {
            lobbyPanel.SetActive(_var);
        }

        #endregion Lobby Panel

        #region Loading

        public async void LoadingMainMenu(string _email)
        {
            // Total Loading Steps 4 =>  100 / 4 = 25
            int loading = 0;
            loadingBar.value = loading;
            loadingPercentage.text = loading + "%";

            authCanvas.SetActive(false);
            mainMenuLoadingCanvas.SetActive(true);
            loading += 25;
            loadingBar.value = loading;
            loadingPercentage.text = loading + "%";

            await firestore.GetUserFromEmail(_email);
            loading += 25;
            loadingBar.value = loading;
            loadingPercentage.text = loading + "%";

            loadingUITask = false;
            firestore.UpdateLocalAccountListener();
            await WaitUntilCondition(() => loadingUITask == true);
            firestore.StateChange(true);
            loading += 25;
            loadingBar.value = loading;
            loadingPercentage.text = loading + "%";

            await Task.Delay(1000);
            loading += 25;
            loadingBar.value = loading;
            loadingPercentage.text = loading + "%";

            await Task.Delay(1000);
            mainMenuLoadingCanvas.SetActive(false);
            mainMenuCanvas.SetActive(true);
        }

        private async Task WaitUntilCondition(Func<bool> condition)
        {
            while (!condition())
            {
                // Delay for a short period before checking the condition again
                await Task.Delay(100);
            }
        }

        #endregion Loading

        #region Update UI

        public async void UpdateUI()
        {
            Debug.Log("Callback");
            await UpdateProfilePanel();
            await UpdateFriendList();
            await UpdateSentFriendRequestsPanel();
            await UpdateFriendRequestPanel();
            await UpdateInviteToLobbyPanel();
            loadingUITask = true;
        }

        private Task UpdateProfilePanel()
        {
            currentUserName.text = firestore.accountFirebase.User;
            playerIcon.sprite = playerIcons[firestore.accountFirebase.PlayerIcon];
            winrate.text = firestore.accountFirebase.Winrate.ToString() + " %";
            rankPoints.text = firestore.accountFirebase.RankPoints.ToString();
            return Task.CompletedTask;
        }

        private async Task UpdateFriendList()
        {
            specialFunctions.DestroyChildrenOf(friendScrollViewContent);

            if (firestore.accountFirebase.FriendList.Count != 0)
            {
                noFriend.SetActive(false);
                GameObject friendHeader = Instantiate(friendHeaderPrefab, friendScrollViewContent.transform);

                int onlinePlayers = 0;
                foreach (string friendId in firestore.accountFirebase.FriendList)
                {
                    AccountFirebase account = await firestore.GetAccountFromId(friendId);
                    GameObject friend = Instantiate(friendPrefab, friendScrollViewContent.transform);
                    friend.name = account.Id;
                    TMP_Text[] textComponent = friend.GetComponentsInChildren<TMP_Text>();
                    textComponent[0].text = account.User;
                    Image imgComponent = friend.GetComponentInChildren<Image>();
                    imgComponent.sprite = playerIcons[account.PlayerIcon];

                    bool onlineFound = false;

                    foreach (string friendStatusId in firestore.accountFirebase.OnlineFriends)
                    {
                        if (friendId == friendStatusId)
                        {
                            textComponent[1].text = "Online";
                            onlinePlayers++;
                            onlineFound = true;
                            break;
                        }
                    }
                    if (onlineFound == false)
                    {
                        foreach (string friendStatusId in firestore.accountFirebase.OfflineFriends)
                        {
                            if (friendId == friendStatusId)
                            {
                                textComponent[1].text = "Offline";
                                break;
                            }
                        }
                    }
                }
                TMP_Text totalFriend = friendHeader.GetComponentInChildren<TMP_Text>();
                totalFriend.text = "( " + onlinePlayers + " / " + firestore.accountFirebase.FriendList.Count.ToString() + " )";
            }
            else
            {
                noFriend.SetActive(true);
            }
        }

        private async Task UpdateSentFriendRequestsPanel()
        {
            specialFunctions.DestroyChildrenOf(sentFriendRequestScrollViewContent);

            if (firestore.accountFirebase.SentFriendRequests.Count != 0)
            {
                noSentFriendRequest.SetActive(false);
                foreach (string friendId in firestore.accountFirebase.SentFriendRequests)
                {
                    AccountFirebase account = await firestore.GetAccountFromId(friendId);
                    GameObject instantiatedPrefab = Instantiate(sentFriendPrefab, sentFriendRequestScrollViewContent.transform);
                    TMP_Text textComponent = instantiatedPrefab.GetComponentInChildren<TMP_Text>();
                    textComponent.text = account.User;
                    Image imageComponent = instantiatedPrefab.GetComponentInChildren<Image>();
                    imageComponent.sprite = playerIcons[account.PlayerIcon];
                }
            }
            else
            {
                noSentFriendRequest.SetActive(true);
            }
        }

        private async Task UpdateFriendRequestPanel()
        {
            if (firestore.accountFirebase.FriendRequestsList.Count != 0)
            {
                specialFunctions.DestroyChildrenOf(friendRequestScrollContent);
                EnableFriendRequestButton(true);
                foreach (string friendId in firestore.accountFirebase.FriendRequestsList)
                {
                    AccountFirebase account = await firestore.GetAccountFromId(friendId);
                    GameObject instantiatedPrefab = Instantiate(friendRequestPrefab, friendRequestScrollContent.transform);
                    instantiatedPrefab.name = account.Id;
                    TMP_Text textComponent = instantiatedPrefab.GetComponentInChildren<TMP_Text>();
                    textComponent.text = account.User;
                    Image imageComponent = instantiatedPrefab.GetComponentInChildren<Image>();
                    imageComponent.sprite = playerIcons[account.PlayerIcon];
                }

                friendRequestsNumber1.text = firestore.accountFirebase.FriendRequestsList.Count.ToString();
                friendRequestsNumber2.text = "(" + firestore.accountFirebase.FriendRequestsList.Count.ToString() + ")";
            }
            else
            {
                specialFunctions.DestroyChildrenOf(friendRequestScrollContent);
                EnableFriendRequestButton(false);
                EnableFriendRequestPanel(false);
            }
        }

        public async Task UpdateInviteToLobbyPanel()
        {
            if (firestore.accountFirebase.InviteToGameList == null)
            {
                specialFunctions.DestroyChildrenOf(inviteToLobbyPanel);
                inviteToLobbyPanel.SetActive(false);
            }
            else if (firestore.accountFirebase.InviteToGameList.Count != 0)
            {
                inviteToLobbyPanel.SetActive(true);
                foreach (string inviteId in firestore.accountFirebase.InviteToGameList)
                {
                    AccountFirebase account = await firestore.GetAccountFromId(inviteId);
                    GameObject lobbyInvite = Instantiate(lobbyInvitePrefab, inviteToLobbyScrollViewContent.transform);
                    lobbyInvite.name = account.Id;
                    TMP_Text textComponent = lobbyInvite.GetComponentInChildren<TMP_Text>();
                    textComponent.text = account.User;
                    Image imageComponent = lobbyInvite.GetComponentInChildren<Image>();
                    imageComponent.sprite = playerIcons[account.PlayerIcon];
                }
            }
        }

        #endregion Update UI

        #region Setting Panel

        public void EnableSettingsPanel(bool _var)
        {
            settingCanvas.SetActive(_var);
        }

        #endregion Setting Panel

        #region Buttons

        public void EnableFriendRequestButton(bool _val)
        {
            friendRequestButtonPanel.SetActive(_val);
        }

        public void SendFriendRequest()
        {
            firestore.SendFriendRequest(specialFunctions.UpperCase(addFriendInput.text));
        }

        public void MinimizeClientButton()
        {
            EventSystem.current.SetSelectedGameObject(null);
            BorderlessWindow.MinimizeWindow();
        }

        public void ExitClientButton()
        {
            EventSystem.current.SetSelectedGameObject(null);
            Application.Quit();
        }

        public void UpdateUserIcon(int _var)
        {
            firestore.UpdateUserIcon(_var);
        }

        public void EnablePlayButton(bool _var)
        {
            playButton.SetActive(_var);
        }

        public void EnableStartButton(bool _var)
        {
            startButton.SetActive(_var);
        }

        public void AwaitStartButton(bool _var)
        {
            startButton.GetComponent<Button>().interactable = !_var;
        }

        #endregion Buttons
    }
}