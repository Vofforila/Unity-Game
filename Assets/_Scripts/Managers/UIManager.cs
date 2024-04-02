using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using Database;
using UnityEngine.UI;
using Fusion;
using System.Collections.Generic;
using System.Linq;
using static UI.GameUIManager;
using Data;
using UnityEngine.EventSystems;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("LocalData")]
        [SerializeField] private Firestore firestore;
        [SerializeField] private LocalData localdata;

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

        [Header("Friend Request Panel")]
        [SerializeField] private GameObject friendRequestButtonPanel;
        [SerializeField] private GameObject friendRequestPanel;
        [SerializeField] private GameObject friendRequestScrollContent;
        [SerializeField] private TMP_Text friendRequestsNumber1;
        [SerializeField] private TMP_Text friendRequestsNumber2;

        [Header("Friend List")]
        [SerializeField] private GameObject friendScrollViewContent;
        [SerializeField] private TMP_Text friendCount;

        [Header("InviteToLobbyPanel")]
        [SerializeField] private GameObject inviteToLobbyPanel;
        [SerializeField] private GameObject inviteToLobbyScrollViewContent;

        [Header("Lobby")]
        [SerializeField] private GameObject playButton;
        [SerializeField] private GameObject startButton;
        [SerializeField] private GameObject lobbyPanel;
        [SerializeField] private GameObject homePanel;
        [SerializeField] private GameObject chatBoxPanel;
        [SerializeField] private GameObject gameUICanvasPrefab;

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

        // Close Everything
        private void Awake()
        {
            Instance = this;
            authCanvas.SetActive(true);
            mainMenuLoadingCanvas.SetActive(false);
            mainMenuCanvas.SetActive(false);
            popupCanvas.SetActive(true);
            OpenLoginPanel();
        }

        private void Start()
        {
            if (localdata.currentLvl > 0)
            {
                ShowStatisticPanelEvent();
                localdata.currentLvl = 0;
            }
            firestore.TestFunction();
        }

        ////////////////////////////////////
        // Open/Close Canvases
        ////////////////////////////////////
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

        ////////////////////////////////////
        // Pop-ups
        ////////////////////////////////////

        public void EnableFriendRequestPanel(bool _val)
        {
            friendRequestPanel.SetActive(_val);
        }

        public void EnableAddFriendPanel(bool _var)
        {
            addFriendPanel.SetActive(_var);
        }

        ////////////////////////////////////
        // Choose Icon Panel
        ////////////////////////////////////

        public void EnableChooseIcons(bool _var)
        {
            chooseIconPanel.SetActive(_var);
        }

        ////////////////////////////////////
        // Friend Panel
        ////////////////////////////////////

        public void EnableFriendRequestButton(bool _val)
        {
            friendRequestButtonPanel.SetActive(_val);
        }

        ////////////////////////////////////
        // Statistics Panel
        ////////////////////////////////////

        public async void ShowStatisticPanelEvent()
        {
            matchStatisticsPanel.SetActive(true);

            Dictionary<PlayerRef, PlayerData> playerDictionary = GameUIManager.Instance.PlayerDictionary;

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

        ////////////////////////////////////
        // Lobby Panel
        ////////////////////////////////////

        public void EnableHomePanel(bool _var)
        {
            homePanel.SetActive(_var);
        }

        public void EnableLobbyPanel(bool _var)
        {
            lobbyPanel.SetActive(_var);
        }

        public void EnableChatBoxPanel(bool _var)
        {
            chatBoxPanel.SetActive(_var);
        }

        ////////////////////////////////////
        // LoadingMenu
        ////////////////////////////////////
        public async void LoadingMainMenu(string _email)
        {
            // Total Loading Steps 4 =>  100 / 4 = 25
            int loading = 0;
            loadingBar.value = loading;
            loadingPercentage.text = loading + "%";
            loginPanel.SetActive(false);
            mainMenuLoadingCanvas.SetActive(true);
            loading += 25;
            loadingBar.value = loading;
            loadingPercentage.text = loading + "%";

            await firestore.GetUserFromEmail(_email);
            loading += 25;
            loadingBar.value = loading;
            loadingPercentage.text = loading + "%";

            firestore.UpdateAccountFirebase();
            loading += 25;
            loadingBar.value = loading;
            loadingPercentage.text = loading + "%";

            friendRequestPanel.SetActive(false);
            addFriendPanel.SetActive(false);

            await Task.Delay(1000);
            loading += 25;
            loadingBar.value = loading;
            loadingPercentage.text = loading + "%";
            await Task.Delay(1000);

            mainMenuLoadingCanvas.SetActive(false);
            mainMenuCanvas.SetActive(true);
        }

        ////////////////////////////////////
        // Settings Panel
        ////////////////////////////////////

        public void EnableSettingsPanel(bool _var)
        {
            settingCanvas.SetActive(_var);
        }

        ////////////////////////////////////
        // Update UI
        ////////////////////////////////////
        public void UpdateUI()
        {
            Debug.Log("Callback");
            UpdateProfilePanel();
            UpdateFriendList();
            UpdateAddFriendPanel();
            UpdateFriendRequestPanel();
            UpdateInviteToLobbyPanel();
        }

        public void UpdateProfilePanel()
        {
            currentUserName.text = firestore.accountFirebase.User;
            playerIcon.sprite = playerIcons[firestore.accountFirebase.PlayerIcon];
            winrate.text = firestore.accountFirebase.Winrate.ToString() + " %";
            rankPoints.text = firestore.accountFirebase.RankPoints.ToString();
        }

        public void UpdateFriendList()
        {
            DestroyChildrenOf(friendScrollViewContent);
            if (firestore.accountFirebase.FriendList[0].ToString() == "Default")
            {
                // Make panel for 0
            }
            else
            {
                foreach (object friend in firestore.accountFirebase.FriendList)
                {
                    GameObject instantiatedPrefab = Instantiate(friendPrefab, friendScrollViewContent.transform);
                    TMP_Text textComponent = instantiatedPrefab.GetComponentInChildren<TMP_Text>();
                    textComponent.text = friend.ToString();
                }
            }
        }

        public void UpdateAddFriendPanel()
        {
            DestroyChildrenOf(sentFriendRequestScrollViewContent);
            if (firestore.accountFirebase.SentFriendRequests == null)
            {
                // Make Pannel for 0 friends
            }
            if (firestore.accountFirebase.SentFriendRequests.Count != 1)
            {
                foreach (object friend in firestore.accountFirebase.SentFriendRequests)
                {
                    GameObject instantiatedPrefab = Instantiate(friendPrefab, sentFriendRequestScrollViewContent.transform);
                    TMP_Text textComponent = instantiatedPrefab.GetComponentInChildren<TMP_Text>();
                    textComponent.text = friend.ToString();
                }
            }
        }

        public void UpdateFriendRequestPanel()
        {
            if (firestore.accountFirebase.FriendRequestsList == null)
            {
                DestroyChildrenOf(friendRequestScrollContent);
                EnableFriendRequestButton(false);
                EnableFriendRequestPanel(false);
            }
            else if (firestore.accountFirebase.FriendRequestsList.Count != 0)
            {
                DestroyChildrenOf(friendRequestScrollContent);
                EnableFriendRequestButton(true);
                foreach (KeyValuePair<int, object> friendRequest in firestore.accountFirebase.FriendRequestsList)
                {
                    // Create the new Friend Request Objects
                    GameObject instantiatedPrefab = Instantiate(friendRequestPrefab, friendRequestScrollContent.transform);
                    TMP_Text textComponent = instantiatedPrefab.GetComponentInChildren<TMP_Text>();
                    textComponent.text = friendRequest.Value.ToString();
                }

                friendRequestsNumber1.text = firestore.accountFirebase.FriendRequestsList.Count.ToString();
                friendRequestsNumber2.text = "(" + firestore.accountFirebase.FriendRequestsList.Count.ToString() + ")";
            }
        }

        public void UpdateInviteToLobbyPanel()
        {
            if (firestore.accountFirebase.InviteToGameList == null)
            {
                DestroyChildrenOf(inviteToLobbyPanel);
                inviteToLobbyPanel.SetActive(false);
            }
            else if (firestore.accountFirebase.InviteToGameList.Count != 0)
            {
                inviteToLobbyPanel.SetActive(true);
                foreach (object inviteToGame in firestore.accountFirebase.InviteToGameList)
                {
                    // Create the new Friend Request Objects
                    GameObject instantiatedPrefab = Instantiate(lobbyInvitePrefab, inviteToLobbyScrollViewContent.transform);
                    TMP_Text textComponent = instantiatedPrefab.GetComponentInChildren<TMP_Text>();
                    textComponent.text = inviteToGame.ToString();
                }
            }
        }

        ////////////////////////////////////
        // Button Functions
        ////////////////////////////////////
        public void SendFriendRequest()
        {
            firestore.SendFriendRequest(addFriendInput.text);
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

        ////////////////////////////////////
        // NavBar
        ////////////////////////////////////

        public void EnablePlayButton(bool _var)
        {
            playButton.SetActive(_var);
        }

        public void EnableStartButton(bool _var)
        {
            startButton.SetActive(_var);
        }

        // Function to destroy objects
        public void DestroyChildrenOf(GameObject targetObject)
        {
            foreach (Transform child in targetObject.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}