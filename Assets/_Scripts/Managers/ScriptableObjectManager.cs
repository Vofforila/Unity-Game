using Data;
using UnityEngine;

public class ScriptableObjectManager : MonoBehaviour
{
    [SerializeField] private LocalData localData;

    private void Awake()
    {
        localData.currentLvl = 0;
    }
}