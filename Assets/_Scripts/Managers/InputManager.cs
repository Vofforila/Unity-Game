using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using Server;

namespace PlayerInput
{
    public class InputManager : SimulationBehaviour, INetworkRunnerCallbacks
    {
        private FusionManager fusionManager;
        private NetworkInputData inputData = new();
        private Camera playerCamera;
        private LayerMask targetLayer;

        public void Start()
        {
            fusionManager = FusionManager.Instance;
            fusionManager.runner.AddCallbacks(this);
            targetLayer = LayerMask.GetMask("HitZone");
        }

        public void OnDisable()
        {
            fusionManager.runner.RemoveCallbacks(this);
        }

        // High Ticks
        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            // Reset Input / Server Tick
            // Set Input
            input.Set(inputData);
            inputData = default;
        }

        // Low Ticks
        private void Update()
        {
            // Keys
            inputData.GameButton.Set(GameButton.Z, Input.GetKey(KeyCode.Z));
            inputData.GameButton.Set(GameButton.X, Input.GetKey(KeyCode.X));
            inputData.GameButton.Set(GameButton.C, Input.GetKey(KeyCode.C));
            inputData.GameButton.Set(GameButton.V, Input.GetKey(KeyCode.V));

            if (Input.GetMouseButton(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, targetLayer))
                {
                    inputData.raycast = hit.point;
                }
            }

            // Mouse
            inputData.GameButton.Set(GameButton.LeftClick, Input.GetMouseButton(0));
            inputData.GameButton.Set(GameButton.RightClick, Input.GetMouseButton(1));
            inputData.GameButton.Set(GameButton.MiddleMouse, Input.GetMouseButton(2));
        }

        public void OnConnectedToServer(NetworkRunner runner)
        {
        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
        {
        }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {
        }

        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
        {
        }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
        }

        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
        }

        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
        }

        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
        {
        }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
        {
        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {
        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
        }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
        }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {
        }
    }
}