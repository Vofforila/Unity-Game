using Data;
using Server;
using UnityEngine;

namespace Host
{
    public class GameManager : MonoBehaviour
    {
        [Header("Scriptable")]
        [SerializeField] private LocalData localdaData;

        [Header("GameObjects")]
        [SerializeField] private GameObject fusionManagerPrefab;
        [SerializeField] private GameObject gameUICanvasPrefab;

        private GameObject fusionManager;
        private GameObject gameUICanvas;

        public static GameManager Instance;

        private void Awake()
        {
            Instance = this;
            if (FusionManager.Instance == null)
            {
                CreateServer();
            }
            else
            {
                localdaData.currentLvl = -1;
            }
        }

        public void CreateServer()
        {
            localdaData.currentLvl = 0;
            fusionManager = Instantiate(fusionManagerPrefab);
            fusionManager.name = "FusionManager";
            gameUICanvas = Instantiate(gameUICanvasPrefab);
            gameUICanvas.name = "GameUICanvas";
        }

        public void DestoryServer()
        {
            localdaData.currentLvl = 0;
            Destroy(fusionManager);
            Destroy(gameUICanvas);
            CreateServer();
        }
    }
}