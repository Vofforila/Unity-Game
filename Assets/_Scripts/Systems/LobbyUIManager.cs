using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;

namespace UI
{
    public class LobbyUIManager : MonoBehaviour
    {
        [SerializeField] private GameObject bannerPrefab;
        [SerializeField] private Transform BannerLayoutTransform;

        private Dictionary<PlayerRef, PlayerDictionary> playerDictionaryList;

        public static LobbyUIManager Instance;

        private void Awake()
        {
            Instance = this;
            // Create a new UI List
            playerDictionaryList = new();
        }

        // Create new UI
        public void CreateUI(PlayerRef _player)
        {
            GameObject entry = Instantiate(bannerPrefab, BannerLayoutTransform);

            // Make a new Player UI Object
            PlayerDictionary playerDictionary = new();

            playerDictionary.PlayerUI = entry;

            // Attach the Player UI Object to a List
            playerDictionaryList.Add(_player, playerDictionary);

            // Update UI
            UpdateUI(_player);
        }

        public void UpdateUserName(PlayerRef _player, string _username)
        {
            playerDictionaryList[_player].UserName = _username;

            UpdateUI(_player);
        }

        public void RemovePlayer(PlayerRef _player)
        {
            GameObject PlayerUI = playerDictionaryList[_player].PlayerUI;

            Destroy(PlayerUI);
            playerDictionaryList.Remove(_player);
        }

        public void UpdateUI(PlayerRef _player)
        {
            GameObject PlayerUI = playerDictionaryList[_player].PlayerUI;

            PlayerUI.GetComponentInChildren<TMP_Text>().text = playerDictionaryList[_player].UserName;
        }

        private class PlayerDictionary
        {
            public GameObject PlayerUI { get; set; }

            public string UserName { get; set; }

            public PlayerDictionary()
            {
                UserName = "";
            }
        }
    }
}