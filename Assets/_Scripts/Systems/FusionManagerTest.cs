using Data;
using Firebase.Firestore;
using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UI;
using Database;
using Settings;

namespace Server
{
    public class FusionManagerTest : MonoBehaviour, INetworkRunnerCallbacks

    {
        [Header("Test")]
        public bool test = false;

        [Header("Fusion")]
        public NetworkRunner runner;

        [Header("Scriptable")]
        [SerializeField] private Firestore firestore;
        [SerializeField] private LocalData localData;

        [Header("Settings")]
        [SerializeField] internal WindowScript windowScript;

        [Header("Event")]
        public UnityEvent playLevel1Event;
        public UnityEvent playLevel2Event;
        public UnityEvent playLevel3Event;
        public UnityEvent playLevel4Event;
        public UnityEvent showStatisticEvent;

        // Variables
        [HideInInspector] public List<PlayerRef> playersInGame = new();

        public static FusionManagerTest Instance;

        #region Awake

        private void Awake()
        {
            // Disable firestore duplication
            FirebaseFirestore.DefaultInstance.Settings.PersistenceEnabled = false;
            Instance = this;

            DontDestroyOnLoad(gameObject);
            runner = gameObject.AddComponent<NetworkRunner>();
        }

        #endregion Awake

        #region TestGUI

        // Test GUI
        private void OnGUI()
        {
            if (test == true)
            {
                UIManager managerUi = GameObject.Find("UIManager").GetComponent<UIManager>();
                managerUi.mainMenuCanvas.SetActive(true);
                managerUi.authCanvas.SetActive(false);
                if (GUI.Button(new Rect(Screen.width - 200, 0, 200, 40), "Play"))
                {
                    firestore.accountFirebase.User = "Test1";
                    managerUi.EnablePlayButton(false);
                    managerUi.EnableStartButton(true);
                    managerUi.EnableHomePanel(false);
                    managerUi.EnableLobbyPanel(true);
                    CreateLobby();
                }
                if (GUI.Button(new Rect(Screen.width - 200, 40, 200, 40), "Join"))
                {
                    firestore.accountFirebase.User = "Test2";
                    localData.inviteName = "Test1";
                    managerUi.EnablePlayButton(false);
                    managerUi.EnableStartButton(true);
                    managerUi.EnableHomePanel(false);
                    managerUi.EnableLobbyPanel(true);
                    InviteResponseEvent();
                    test = false;
                }
                if (GUI.Button(new Rect(Screen.width - 200, 80, 200, 40), "Level 1"))
                {
                    LoadLevel1();
                    test = false;
                }
                if (GUI.Button(new Rect(Screen.width - 200, 120, 200, 40), "Level 2"))
                {
                    LoadLevel2Event();
                    test = false;
                }
                if (GUI.Button(new Rect(Screen.width - 200, 160, 200, 40), "Level 3"))
                {
                    LoadLevel3Event();
                    test = false;
                }
                if (GUI.Button(new Rect(Screen.width - 200, 200, 200, 40), "Level 4"))
                {
                    LoadLevel4Event();
                    test = false;
                }
                if (GUI.Button(new Rect(Screen.width - 200, 240, 200, 40), "Join 3"))
                {
                    firestore.accountFirebase.User = "Test3";
                    localData.inviteName = "Test1";
                    managerUi.EnablePlayButton(false);
                    managerUi.EnableStartButton(true);
                    managerUi.EnableHomePanel(false);
                    managerUi.EnableLobbyPanel(true);
                    InviteResponseEvent();
                    test = false;
                }
                if (GUI.Button(new Rect(0, 280, 200, 40), "Close Server"))
                {/*
                    managerUi.EnablePlayButton(true);
                    managerUi.EnableStartButton(false);
                    managerUi.EnableLobbyPanel(false);
                    managerUi.EnableHomePanel(true);*/
                    LeaveLobby();
                }
            }
        }

        #endregion TestGUI

        #region LevelLoading

        public async void LoadMainMenuEvent()
        {
            Debug.Log("Callback");
            await runner.LoadScene(SceneRef.FromIndex(0), LoadSceneMode.Single);
            localData.currentLvl = 0;
            GameUIManager.Instance.UpdateLevelState(localData.currentLvl);
        }

        public async void LoadLevel1()
        {
            StartCoroutine(SettingManager.Instance.IChangeResolution());
            await runner.LoadScene(SceneRef.FromIndex(1), LoadSceneMode.Single);
            Debug.Log("Play Level 1 - Event");
            playLevel1Event.Invoke();
        }

        public async void LoadLevel2Event()
        {
            Debug.Log("Callback");
            await runner.LoadScene(SceneRef.FromIndex(2), LoadSceneMode.Single);
            Debug.Log("Play Level 2 - Event");
            playLevel2Event.Invoke();
        }

        public async void LoadLevel3Event()
        {
            Debug.Log("Callback");
            await runner.LoadScene(SceneRef.FromIndex(3), LoadSceneMode.Single);
            Debug.Log("Play Level 3 - Event");
            playLevel3Event.Invoke();
        }

        public async void LoadLevel4Event()
        {
            Debug.Log("Callback");
            await runner.LoadScene(SceneRef.FromIndex(4), LoadSceneMode.Single);
            Debug.Log("Play Level 4 - Event");
            playLevel4Event.Invoke();
        }

        #endregion LevelLoading

        #region Create/Join Lobby

        public async void CreateLobby()
        {
            localData.playerList = new(); // ???
            localData.currentLvl = 0;
            GameUIManager.Instance.UpdateLevelState(localData.currentLvl);
            await CreateLobbyTask();
        }

        public async Task CreateLobbyTask()
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

            // Create Session
            var result = await runner.StartGame(new StartGameArgs()
            {
                GameMode = GameMode.Host,
                SessionName = firestore.accountFirebase.User,
                Scene = scene,
                SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
            });

            if (result.Ok)
            {
                Debug.Log("Hosted");
            }
            else
            {
                Debug.LogError($"Failed to Start: {result.ShutdownReason}");
            }
        }

        public async void InviteResponseEvent()
        {
            Debug.Log("Callback");
            await JoinSessionTask();
        }

        public async Task JoinSessionTask()
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

            // Join Session
            var result = await runner.StartGame(new StartGameArgs()
            {
                GameMode = GameMode.Client,
                SessionName = localData.inviteName,
                Scene = scene,
                PlayerCount = 4,
                SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
            });

            if (result.Ok)
            {
                Debug.Log("Joined");
            }
            else
            {
                Debug.LogError($"Failed to Start: {result.ShutdownReason}");
            }
        }

        #endregion Create/Join Lobby

        #region Leave Lobby

        public void LeaveLobby()
        {
            if (runner.SessionInfo.Name != firestore.accountFirebase.User)
            {
                runner = null;
            }
            else
            {
                foreach (PlayerRef player in runner.ActivePlayers)
                {
                    runner.Disconnect(player);
                }
                Destroy(runner);
            }
        }

        #endregion Leave Lobby

        #region Fusion API

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

        #endregion Fusion API
    }
}