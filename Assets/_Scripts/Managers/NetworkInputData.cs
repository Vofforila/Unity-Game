using Fusion;
using UnityEngine;

namespace PlayerInput
{
    internal enum GameButton
    {
        LeftClick = 1,
        RightClick = 2,
        MiddleMouse = 3,
        Z = 4,
        X = 5,
        C = 6,
        V = 7,
    }

    public struct NetworkInputData : INetworkInput
    {
        public NetworkButtons GameButton;

        public Vector3 raycast;

        [Header("Level 1 Input")]
        public NetworkBool takeMoney;
        public NetworkBool jump;

        [Header("Level 2 Input")]
        public NetworkBool horseAcceleration;

        [Header("Level 3 Input")]
        public Vector3 clickPosition;
    }
}