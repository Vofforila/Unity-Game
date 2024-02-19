using Data;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using TryhardParty;
using UnityEngine;
using UnityEngine.Events;

namespace Player
{
    public class Level3Manager : MonoBehaviour
    {
        [Header("Scriptable")]
        public LocalData localData;
        public Firestore firestore;

        [Header("Prefab")]
        [SerializeField]
        private NetworkPrefabRef playerPrefab;

        [SerializeField]
        private GameObject cameraPrefab;

        [SerializeField]
        private GameObject catapultPrefab;

        [Header("Events")]
        public UnityEvent loadLevel4Event;

        private List<NetworkObject> instanciatedPlayer;
        private int playersAlive;
        internal FusionManager fusionManager;
        private Transform spawnPoint;
        private List<Transform> spawnPointList;

        /*
        private void Awake()
        {
            fusionManager = GameObject.Find("FusionManager").GetComponent<FusionManager>();
            GameObject.Instantiate(cameraPrefab, new Vector3(0, 15, -15), Quaternion.Euler(45f, 0, 0));
            StartCoroutine(TipPanel());
        }

        public IEnumerator TipPanel()
        {
            yield return new WaitForSecondsRealtime(5f);
            GameObject.Find("GameTip").SetActive(false);
        }

        public void PlayLevel3Event()
        {
            Debug.Log("Callback");
            StartCoroutine(SpawnPlayers());
        }

        public IEnumerator SpawnPlayers()
        {
            StartCoroutine(PlayLevel3());

            instanciatedPlayer = new();
            playersAlive = 4;

            // Get SpawnPoints
            spawnPoint = GameObject.Find("SpawnPoints").transform;
            spawnPointList = new();
            for (int i = 0; i < spawnPoint.childCount; i++)
                spawnPointList.Add(spawnPoint.GetChild(i));

            // Spawn Player
            for (int i = 0; i < localData.playerListData.Count; i++)
            {
                PlayerData playerData = localData.playerListData[i];
                NetworkObject networkPlayer = fusionManager.runner.Spawn(playerPrefab, Vector3.zero, Quaternion.identity, playerData.PlayerRef);

                NetworkTransform networkTransform = networkPlayer.GetComponent<NetworkTransform>();
                networkTransform.Teleport(spawnPointList[i].position);

                instanciatedPlayer.Add(networkPlayer);
            }

            spawnPoint = GameObject.Find("CatapultSpawnPoints").transform;
            for (int i = 0; i < spawnPoint.childCount; i++)
            {
                GameObject.Instantiate(catapultPrefab, spawnPoint.GetChild(i).position, spawnPoint.GetChild(i).rotation);
                yield return new WaitForSecondsRealtime(6f);
            }
        }

        public void DestroyPlayerEvent()
        {
            Debug.Log("Callback");
            for (int i = 0; i < localData.playerListData.Count; i++)
            {
                PlayerData playerData = localData.playerListData[i];
                if (playerData.Username == firestore.accountFirebase.User)
                {
                    fusionManager.runner.Despawn(instanciatedPlayer[i]);
                    playersAlive--;
                }
            }
        }

        public IEnumerator PlayLevel3()
        {
            yield return new WaitUntil(() => playersAlive == 0);
            Debug.Log("Load Level 4 - Event");
            loadLevel4Event.Invoke();
        }
            */
    }
}