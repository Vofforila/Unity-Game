using Fusion;
using UnityEngine;
using UI;

namespace Host
{
    public class CoinPrefab : NetworkBehaviour
    {
        private const int coinValue = 50;
        private GameUIListener gameUIListener;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && Object != null)
            {
                gameUIListener = other.gameObject.GetComponent<GameUIListener>();
                gameUIListener.AddScore(coinValue);

                Runner.Despawn(Object);
            }
        }
    }
}