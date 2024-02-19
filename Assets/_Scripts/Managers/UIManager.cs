using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using Firebase.Firestore;
using Firebase.Extensions;

namespace TryhardParty
{
    public class UIManager : MonoBehaviour
    {
        [Header("LocalData")]
        [SerializeField] public Firestore firestore;

        [Header("Prefabs")]
        [SerializeField] private GameObject friendPrefab;
        [SerializeField] private GameObject friendRequestPrefab;
        [SerializeField] private GameObject lobbyInvitePrefab;

        // Remove public on release
        [Header("Canvases")]
        [SerializeField] public GameObject loginCanvas;
        [SerializeField] private GameObject registerCanvas;
        [SerializeField] private GameObject forgotPasswordCanvas;
        [SerializeField] public GameObject mainMenuLoadingCanvas;
        [SerializeField] public GameObject mainMenuCanvas;

        [Header("ProfilePanel")]
        [SerializeField] private TMP_Text currentUserName;
        [SerializeField] private TMP_Text winrate;
        [SerializeField] private TMP_Text rankPoints;

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

        [Header("InviteToLobbyPanel")]
        [SerializeField] private GameObject inviteToLobbyPanel;
        [SerializeField] private GameObject inviteToLobbyScrollViewContent;

        [Header("Lobby")]
        [SerializeField] private GameObject playButton;
        [SerializeField] private GameObject startButton;
        [SerializeField] private GameObject lobbyPanel;
        [SerializeField] private GameObject homePanel;
        [SerializeField] private GameObject playerBanner1;
        [SerializeField] private GameObject playerBanner2;
        [SerializeField] private GameObject playerBanner3;
        [SerializeField] private GameObject playerBanner4;
        [SerializeField] private GameObject chatBoxPanel;

        [Header("Mach Statistics")]
        [SerializeField] private GameObject matchStatisticsPanel;
        [SerializeField] private GameObject rankPointsGained;
        [SerializeField] public Transform playersStatisticPanel;

        // Close Everything
        private void Awake()
        {
            loginCanvas.SetActive(true);
            registerCanvas.SetActive(false);
            forgotPasswordCanvas.SetActive(false);
            mainMenuLoadingCanvas.SetActive(false);
            mainMenuCanvas.SetActive(false);

            playerBanner1.SetActive(false);
            playerBanner2.SetActive(false);
            playerBanner3.SetActive(false);
            playerBanner4.SetActive(false);
        }

        ////////////////////////////////////
        // Open/Close Canvases
        ////////////////////////////////////
        public void OpenLoginCanvas()
        {
            loginCanvas.SetActive(true);
            registerCanvas.SetActive(false);
            forgotPasswordCanvas.SetActive(false);
            mainMenuLoadingCanvas.SetActive(false);
            mainMenuCanvas.SetActive(false);
        }

        public void OpenRegisterCanvas()
        {
            loginCanvas.SetActive(false);
            registerCanvas.SetActive(true);
            forgotPasswordCanvas.SetActive(false);
            mainMenuLoadingCanvas.SetActive(false);
            mainMenuCanvas.SetActive(false);
        }

        public void OpenForgotPasswordCanvas()
        {
            loginCanvas.SetActive(false);
            registerCanvas.SetActive(false);
            forgotPasswordCanvas.SetActive(true);
            mainMenuLoadingCanvas.SetActive(false);
            mainMenuCanvas.SetActive(false);
        }

        public void OpenMainMenuLoadingCanvas()
        {
            loginCanvas.SetActive(false);
            registerCanvas.SetActive(false);
            forgotPasswordCanvas.SetActive(false);
            mainMenuLoadingCanvas.SetActive(true);
            mainMenuCanvas.SetActive(false);
        }

        public void OpenMainMenuCanvas()
        {
            loginCanvas.SetActive(false);
            registerCanvas.SetActive(false);
            forgotPasswordCanvas.SetActive(false);
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
        // Friend Panel
        ////////////////////////////////////

        public void EnableFriendRequestButton(bool _val)
        {
            friendRequestButtonPanel.SetActive(_val);
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

        public void EnablePlayerBanner1(bool _var)
        {
            playerBanner1.SetActive(_var);
        }

        public void EnablePlayerBanner2(bool _var)
        {
            playerBanner2.SetActive(_var);
        }

        public void EnablePlayerBanner3(bool _var)
        {
            playerBanner3.SetActive(_var);
        }

        public void EnablePlayerBanner4(bool _var)
        {
            playerBanner4.SetActive(_var);
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
            loginCanvas.SetActive(false);
            mainMenuLoadingCanvas.SetActive(true);

            await firestore.GetUserFromEmail(_email);

            firestore.UpdateAccountFirebase();

            friendRequestPanel.SetActive(false);
            addFriendPanel.SetActive(false);
            mainMenuLoadingCanvas.SetActive(false);
            mainMenuCanvas.SetActive(true);
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
            winrate.text = firestore.accountFirebase.Winrate.ToString() + " %";
            rankPoints.text = firestore.accountFirebase.RankPoints.ToString();
        }

        public void UpdateFriendList()
        {
            DestroyChildrenOf(friendScrollViewContent);
            if (firestore.accountFirebase.FriendList.Count != 0)
            {
                foreach (object friend in firestore.accountFirebase.FriendList)
                {
                    GameObject instantiatedPrefab = Instantiate(friendPrefab, friendScrollViewContent.transform);
                    TMP_Text textComponent = instantiatedPrefab.GetComponentInChildren<TMP_Text>();
                    textComponent.text = friend.ToString();
                }
            }
            else
            {
                // Make Pannel for 0 friends
            }
        }

        public void UpdateAddFriendPanel()
        {
            DestroyChildrenOf(sentFriendRequestScrollViewContent);
            if (firestore.accountFirebase.SentFriendRequests.Count != 0)
            {
                foreach (object friend in firestore.accountFirebase.SentFriendRequests)
                {
                    GameObject instantiatedPrefab = Instantiate(friendPrefab, sentFriendRequestScrollViewContent.transform);
                    TMP_Text textComponent = instantiatedPrefab.GetComponentInChildren<TMP_Text>();
                    textComponent.text = friend.ToString();
                }
            }
            else
            {
                // Make Pannel for 0 friendrequests
            }
        }

        public void UpdateFriendRequestPanel()
        {
            if (firestore.accountFirebase.FriendRequestsList.Count != 0)
            {
                DestroyChildrenOf(friendRequestScrollContent);
                EnableFriendRequestButton(true);
                foreach (object friendRequest in firestore.accountFirebase.FriendRequestsList)
                {
                    // Create the new Friend Request Objects
                    GameObject instantiatedPrefab = Instantiate(friendRequestPrefab, friendRequestScrollContent.transform);
                    TMP_Text textComponent = instantiatedPrefab.GetComponentInChildren<TMP_Text>();
                    textComponent.text = friendRequest.ToString();
                }

                friendRequestsNumber1.text = firestore.accountFirebase.FriendRequestsList.Count.ToString();
                friendRequestsNumber2.text = "(" + firestore.accountFirebase.FriendRequestsList.Count.ToString() + ")";
            }
            else
            {
                DestroyChildrenOf(friendRequestScrollContent);
                EnableFriendRequestButton(false);
                EnableFriendRequestPanel(false);
            }
        }

        public void UpdateInviteToLobbyPanel()
        {
            if (firestore.accountFirebase.InviteToGameList.Count != 0)
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
            else
            {
                DestroyChildrenOf(inviteToLobbyPanel);
                inviteToLobbyPanel.SetActive(false);
            }
        }

        ////////////////////////////////////
        // Button Functions
        ////////////////////////////////////
        public void SendFriendRequest()
        {
            firestore.SendFriendRequest(addFriendInput.text);
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