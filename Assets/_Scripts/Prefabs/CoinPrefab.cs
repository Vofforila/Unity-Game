using Fusion;
using UnityEngine;

namespace TryhardParty
{
    public class CoinPrefab : NetworkBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Runner.Despawn(this.GetComponent<NetworkObject>());
            }
        }
    }
}