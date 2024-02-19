using Fusion;
using UnityEngine;

internal enum GameButton
{
    Z = 1,
    X = 2,
}

public struct NetworkInputData : INetworkInput
{
    public NetworkButtons GameButton;

    [Header("Level 1 Input")]
    public NetworkBool takeMoney;
    public NetworkBool jump;

    [Header("Level 2 Input")]
    public NetworkBool horseAcceleration;

    [Header("Level 3 Input")]
    public Vector3 clickPosition;
}