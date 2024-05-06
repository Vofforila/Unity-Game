using Data;
using Database;
using Firebase.Firestore;
using Fusion;
using Fusion.Sockets;
using Settings;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Server
{
    public class FusionManager : MonoBehaviour, INetworkRunnerCallbacks
    {
        [Header("Test")]
        public bool test = false;

        [Header("Fusion")]
        [HideInInspector] public NetworkRunner runner;

        [Header("Scriptable")]
        [SerializeField] private Firestore firestore;
        [SerializeField] private LocalData localData;

        [Header("Event")]
        public UnityEvent playLevel1Event;
        public UnityEvent playLevel2Event;
        public UnityEvent playLevel3Event;
        public UnityEvent playLevel4Event;

        [HideInInspector]
        public float timePlayed = 0;

        public static FusionManager Instance;

        #region Awake

        private void Awake()
        {
            localData.currentLvl = 0;
            timePlayed = 0;
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
                if (GUI.Button(new Rect(0, 0, 200, 40), "Play"))
                {
                    firestore.accountFirebase.User = "Test1";
                    managerUi.EnablePlayButton(false);
                    managerUi.EnableStartButton(true);
                    managerUi.EnableHomePanel(false);
                    managerUi.EnableLobbyPanel(true);
                    CreateLobby();
                }
                if (GUI.Button(new Rect(0, 40, 200, 40), "Join"))
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
                if (GUI.Button(new Rect(0, 80, 200, 40), "Level 1"))
                {
                    LoadLevel1();
                    test = false;
                }
                if (GUI.Button(new Rect(0, 120, 200, 40), "Level 2"))
                {
                    LoadLevel2Event();
                    test = false;
                }
                if (GUI.Button(new Rect(0, 160, 200, 40), "Level 3"))
                {
                    LoadLevel3Event();
                    test = false;
                }
                if (GUI.Button(new Rect(0, 200, 200, 40), "Level 4"))
                {
                    LoadLevel4Event();
                    test = false;
                }
                if (GUI.Button(new Rect(0, 240, 200, 40), "Join 3"))
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
            localData.currentLvl = -1;
            await runner.LoadScene(SceneRef.FromIndex(0), LoadSceneMode.Single);
            GameUIManager.Instance.UpdateLevelState(localData.currentLvl);
        }

        public async void LoadLevel1()
        {
            Debug.Log("Callback");
            StartCoroutine(SettingManager.Instance.IChangeResolution(true));
            await runner.LoadScene(SceneRef.FromIndex(1), LoadSceneMode.Single);
            Debug.Log("<color=green>Play Level 1 - Event</color>");
            playLevel1Event.Invoke();
        }

        public async void LoadLevel2Event()
        {
            Debug.Log("Callback");
            await runner.LoadScene(SceneRef.FromIndex(2), LoadSceneMode.Single);
            Debug.Log("<color=green>Play Level 2 - Event</color>");
            playLevel2Event.Invoke();
        }

        public async void LoadLevel3Event()
        {
            Debug.Log("Callback");
            await runner.LoadScene(SceneRef.FromIndex(3), LoadSceneMode.Single);
            Debug.Log("<color=green>Play Level 3 - Event</color>");
            playLevel3Event.Invoke();
        }

        public async void LoadLevel4Event()
        {
            Debug.Log("Callback");
            await runner.LoadScene(SceneRef.FromIndex(4), LoadSceneMode.Single);
            Debug.Log("<color=green>Play Level 4 - Event</color>");
            playLevel4Event.Invoke();
        }

        #endregion LevelLoading

        #region Create/Join Lobby

        public async void CreateLobby()
        {
            Debug.Log("Callback");
            localData.currentLvl = 0;
            UIManager.Instance.AwaitStartButton(true);
            GameUIManager.Instance.UpdateLevelState(localData.currentLvl);
            await CreateLobbyTask();
            UIManager.Instance.AwaitStartButton(false);
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
                Debug.Log("<color=blue>Hosted</color>");
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
            UIManager.Instance.EnableStartButton(false);
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

        #region TimePlayed

        private void Update()
        {
            if (localData.currentLvl >= 1)
            {
                timePlayed++;
            }
        }

        #endregion TimePlayed

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
            Debug.Log("<color=blue>2</color>");
        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
        {
            Debug.Log("<color=blue>3</color>");
        }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {
            Debug.Log("<color=blue>4</color>");
        }

        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
        {
            Debug.Log("<color=blue>5</color>");
        }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
            Debug.Log("<color=blue>6</color>");
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
        }

        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
            Debug.Log("<color=blue>9</color>");
        }

        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
            Debug.Log("<color=blue>10</color>");
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            Debug.Log("<color=blue>11</color>");
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            Debug.Log("<color=blue>12</color>");
        }

        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
        {
            Debug.Log("<color=blue>13</color>");
        }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
        {
            Debug.Log("<color=blue>14</color>");
        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {
        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
        }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
            Debug.Log("<color=blue>17</color>");
        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            Debug.Log("<color=blue>18</color>");
        }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {
            Debug.Log("<color=blue>19</color>");
        }

        #endregion Fusion API
    }
}