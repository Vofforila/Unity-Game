using Data;
using Firebase.Firestore;
using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace TryhardParty
{
    public class FusionManager : MonoBehaviour, INetworkRunnerCallbacks

    {
        [Header("Test")]
        public bool test = false;

        [Header("Fusion")]
        [HideInInspector]
        public NetworkRunner runner;

        [Header("Scriptable")]
        public Firestore firestore;
        public LocalData localData;

        [SerializeField] private NetworkPrefabRef lobbyManagerPrefab;
        [SerializeField] private NetworkPrefabRef lobbyManagerTestPrefab;

        [Header("Event")]
        public UnityEvent playLevel1Event;
        public UnityEvent playLevel2Event;
        public UnityEvent playLevel3Event;
        public UnityEvent playLevel4Event;

        // Variables
        [HideInInspector]
        public List<PlayerRef> playersInGame = new();

        private void Awake()
        {
            // Disable firestore duplication
            FirebaseFirestore.DefaultInstance.Settings.PersistenceEnabled = false;

            DontDestroyOnLoad(gameObject);
            runner = gameObject.AddComponent<NetworkRunner>();

            localData.playerList = new();
        }

        // Test GUI
        private void OnGUI()
        {
            if (test == true)
            {
                UIManager managerUi = GameObject.Find("UIManager").GetComponent<UIManager>();
                managerUi.mainMenuCanvas.SetActive(true);
                managerUi.loginCanvas.SetActive(false);
                if (GUI.Button(new Rect(0, 0, 200, 40), "Play"))
                {
                    firestore.accountFirebase.User = "Test1";
                    managerUi.EnablePlayButton(false);
                    managerUi.EnableStartButton(true);
                    managerUi.EnableHomePanel(false);
                    CreateLobby();
                }

                if (GUI.Button(new Rect(0, 40, 200, 40), "Join"))
                {
                    firestore.accountFirebase.User = "Test2";
                    localData.inviteName = "Test1";
                    managerUi.EnablePlayButton(false);
                    managerUi.EnableStartButton(true);
                    managerUi.EnableHomePanel(false);
                    test = false;
                    JoinLobby();
                }
                if (GUI.Button(new Rect(0, 80, 200, 40), "Start Level 1"))
                {
                    test = false;
                    LoadLevel1();
                }
                if (GUI.Button(new Rect(0, 120, 200, 40), "Start Level 2"))
                {
                    test = false;
                    LoadLevel2Event();
                }
                if (GUI.Button(new Rect(0, 160, 200, 40), "Start Level 3"))
                {
                    test = false;
                    LoadLevel3Event();
                }
                if (GUI.Button(new Rect(0, 200, 200, 40), "Start Level 4"))
                {
                    test = false;
                    LoadLevel4Event();
                }
                if (GUI.Button(new Rect(0, 240, 200, 40), "3rd Player"))
                {
                    firestore.accountFirebase.User = "Test3";
                    localData.inviteName = "Test1";
                    managerUi.EnablePlayButton(false);
                    managerUi.EnableStartButton(true);
                    managerUi.EnableHomePanel(false);
                    test = false;
                    JoinLobby();
                }

                if (GUI.Button(new Rect(0, 280, 200, 40), "Leave"))
                {
                    managerUi.EnablePlayButton(true);
                    managerUi.EnableStartButton(false);
                    managerUi.EnableLobbyPanel(false);
                    managerUi.EnableHomePanel(true);
                    test = false;
                }
            }
        }

        public async void LoadLevel1()
        {
            localData.currentLvl = 1;
            await runner.LoadScene(SceneRef.FromIndex(1), LoadSceneMode.Single);
            Debug.Log("Play Level 1 - Event");
            playLevel1Event.Invoke();
        }

        public async void LoadLevel2Event()
        {
            Debug.Log("Callback");
            localData.currentLvl = 2;
            await runner.LoadScene(SceneRef.FromIndex(2), LoadSceneMode.Single);
            Debug.Log("Play Level 2 - Event");
            playLevel2Event.Invoke();
        }

        public async void LoadLevel3Event()
        {
            Debug.Log("Callback");
            localData.currentLvl = 3;
            await runner.LoadScene(SceneRef.FromIndex(3), LoadSceneMode.Single);
            Debug.Log("Play Level 3 - Event");
            playLevel3Event.Invoke();
        }

        public async void LoadLevel4Event()
        {
            Debug.Log("Callback");
            localData.currentLvl = 4;
            await runner.LoadScene(SceneRef.FromIndex(4), LoadSceneMode.Single);
            Debug.Log("Play Level 4 - Event");
            playLevel4Event.Invoke();
        }

        public async void LoadMainMenu()
        {
            Debug.Log("Callback");
            localData.showMatchStatistics = true;
            await runner.LoadScene(SceneRef.FromIndex(0), LoadSceneMode.Single);
        }

        public void InviteResponseEvent()
        {
            Debug.Log("Callback");
            if (localData.inviteResponse == true)
            {
                //firestore.RemoveInvite();
                JoinLobby();
            }
            else
            {
                firestore.RemoveInvite();
            }
        }

        public async void CreateLobby()
        {
            if (runner == null)
            {
                runner = gameObject.AddComponent<NetworkRunner>();
            }

            localData.playerList = new();

            await CreateLobbyTask(runner);
        }

        public async Task CreateLobbyTask(NetworkRunner runner)
        {
            // Create the Fusion runner and let it know that we will be providing user input
            if (runner == null)
            {
                runner = gameObject.AddComponent<NetworkRunner>();
            }
            runner.ProvideInput = true;

            // Create the NetworkSceneInfo from the current scene
            var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
            var sceneInfo = new NetworkSceneInfo();
            if (scene.IsValid)
            {
                sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
            }

            var result = await runner.StartGame(new StartGameArgs()
            {
                GameMode = GameMode.Host,
                SessionName = firestore.accountFirebase.User,
                Scene = scene,
                SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
            });

            if (result.Ok)
            {
                if (Debug.isDebugBuild)
                    Debug.Log("Hosted");
            }
            else
            {
                if (Debug.isDebugBuild)
                    Debug.LogError($"Failed to Start: {result.ShutdownReason}");
            }
        }

        public async void JoinLobby()
        {
            // firestoreManager.JoinLobby(invitedFriend);
            if (runner == null)
            {
                runner = gameObject.AddComponent<NetworkRunner>();
            }

            await JoinSessionTask(runner);
        }

        public async Task JoinSessionTask(NetworkRunner runner)
        {
            // Create the Fusion runner and let it know that we will be providing user input
            if (runner == null)
            {
                runner = gameObject.AddComponent<NetworkRunner>();
            }
            runner.ProvideInput = true;

            // Create the NetworkSceneInfo from the current scene
            var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
            var sceneInfo = new NetworkSceneInfo();
            if (scene.IsValid)
            {
                sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
            }
            var result = await runner.StartGame(new StartGameArgs()
            {
                GameMode = GameMode.Client,
                SessionName = localData.inviteName,
                Scene = scene,
                PlayerCount = 4,
                SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
            });

            // canvasManager.OpenLobbyPanel();

            if (result.Ok)
            {
                if (Debug.isDebugBuild)
                    Debug.Log("Joined");
            }
            else
            {
                if (Debug.isDebugBuild)
                    Debug.LogError($"Failed to Start: {result.ShutdownReason}");
            }
        }

        public void OnConnectedToServer(NetworkRunner runner)
        {
            Debug.Log("1");
        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
            Debug.Log("2");
        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
        {
            Debug.Log("3");
        }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {
            Debug.Log("4");
        }

        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
        {
            Debug.Log("5");
        }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
            Debug.Log("6");
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
            Debug.Log("8");
        }

        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
            Debug.Log("9");
        }

        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
            Debug.Log("10");
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            Debug.Log("11");
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            Debug.Log("12");
        }

        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
        {
            Debug.Log("13");
        }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
        {
            Debug.Log("14");
        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {
            Debug.Log("15");
        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
            Debug.Log("16");
        }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
            Debug.Log("17");
        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            Debug.Log("18");
        }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {
            Debug.Log("19");
        }

        public void SendChatMessage()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                localData.returnKeyPressed = true;
            }
        }
    }
}