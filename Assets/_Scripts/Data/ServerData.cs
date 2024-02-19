using UnityEngine;
using Fusion;

namespace Data
{
    [CreateAssetMenu(fileName = "ServerData", menuName = "Data/ServerData")]
    public class ServerData : ScriptableObject
    {
        public bool playerTurn;
        public PlayerRef player;
    }
}