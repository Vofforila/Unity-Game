using Fusion;
using UnityEngine;

namespace Host
{
    public class CoinPrefab : NetworkBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("Coin");
            if (other.CompareTag("Player"))
            {
                Debug.Log("Coin");
                Runner.Despawn(Object);
            }
        }
    }
}