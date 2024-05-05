using Data;
using Server;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Scriptable")]
    [SerializeField] private LocalData localdaData;

    [Header("GameObjects")]
    [SerializeField] private GameObject fusionManagerPrefab;
    [SerializeField] private GameObject gameUICanvasPrefab;

    private void Awake()
    {
        if (FusionManager.Instance == null)
        {
            localdaData.currentLvl = 0;
            GameObject fusionManager = Instantiate(fusionManagerPrefab);
            fusionManager.name = "FusionManager";
            GameObject gameUICanvas = Instantiate(gameUICanvasPrefab);
            gameUICanvas.name = "GameUICanvas";
        }
        else
        {
            localdaData.currentLvl = -1;
        }
    }
}