using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using TryhardParty;

public class InputManager : SimulationBehaviour, INetworkRunnerCallbacks
{
    [HideInInspector] internal FusionManager fusionManager;
    private NetworkInputData inputData = new();

    public void Start()
    {
        fusionManager = GameObject.Find("FusionManager").GetComponent<FusionManager>();
        fusionManager.runner.AddCallbacks(this);
    }

    public void OnDisable()
    {
        fusionManager.runner.RemoveCallbacks(this);
    }

    // Networked Input
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        // Reset Input / Server Tick

        // Set Input
        input.Set(inputData);
        inputData = default;
    }

    private void Update()
    {
        inputData.GameButton.Set(GameButton.Z, Input.GetKey(KeyCode.Z));

        inputData.GameButton.Set(GameButton.X, Input.GetKey(KeyCode.X));
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