using Data;
using Database;
using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Test
{
    public class FusionManagerTest : MonoBehaviour, INetworkRunnerCallbacks
    {
        [HideInInspector]
        public NetworkRunner runner;

        [Header("Scriptable")]
        public Firestore firestore;
        public LocalData localData;

        // Test
        public Canvas canvas;

        [SerializeField]
        private NetworkPrefabRef textPrefab;

        [SerializeField]
        private NetworkPrefabRef playerPrefab;

        [Networked] public NetworkString<_16> NickName { get; set; }

        private void Awake()
        {
            runner = gameObject.AddComponent<NetworkRunner>();
        }

        private void OnGUI()
        {
            if (GUI.Button(new Rect(0, 0, 200, 40), "Host"))
            {
                firestore.accountFirebase.User = "Test1";
                CreateLobby();
            }
            if (GUI.Button(new Rect(0, 40, 200, 40), "Join"))
            {
                firestore.accountFirebase.User = "Test2";
                /*localData.inviteName = "Test1";*/
                JoinLobby();
            }
        }

        [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
        private void RpcSetNickName(string nickName)
        {
            if (string.IsNullOrEmpty(nickName)) return;
            NickName = nickName;
        }

        // If Enabled remake invite
        /*   public void InviteResponseEvent()
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
   */

        public async void CreateLobby()
        {
            if (runner == null)
            {
                runner = gameObject.AddComponent<NetworkRunner>();
            }
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
                SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
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
    }
}