using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.UI;
using System;
using TMPro;
using Server;
using UI;

namespace Settings
{
    public class SettingManager : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] internal WindowScript windowScript;

        [Header("Setting Panel Objects")]
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider soundSlider;
        [SerializeField] private TMP_Dropdown gameResolutionDropDown;
        [SerializeField] private TMP_Dropdown clientResolutionDropDown;

        [Header("Setting Properties")]
        public int gameWidth;
        public int gameHeight;
        public int clientWidth;
        public int clientHeight;
        public float music;
        public float sound;
        public int gameResolutionDropDownValue;
        public int clientResolutionDropDownValue;

        //Default Values
        private const int defaultgameWidth = 1920;
        private const int defaultgameHeight = 1080;
        private const int defaultclientWidth = 1600;
        private const int defaultclientHeight = 900;
        private const float defaultmusic = 100f;
        private const float defaultsound = 100f;
        private const int defaultgameResolutionDropDownValue = 0;
        private const int defaultclientResolutionDropDownValue = 0;

        private void Start()
        {
            SettingsData settings = LoadSettings();

            if (settings == null) return;

            // Properties
            gameWidth = settings.gameWidth;
            gameHeight = settings.gameHeight;
            clientWidth = settings.clientWidth;
            clientHeight = settings.clientHeight;

            // Slider
            music = settings.music;
            musicSlider.value = settings.music;
            sound = settings.sound;
            soundSlider.value = settings.sound;

            // DropDown
            gameResolutionDropDown.value = settings.gameResolutionDropDownValue;
            GameResolutionDropDown(settings.gameResolutionDropDownValue);
            clientResolutionDropDown.value = settings.clientResolutionDropDownValue;
            ClientResolutionDropDown(settings.clientResolutionDropDownValue);

            UpdateSettings();
        }

        public void GameResolutionDropDown(int _index)
        {
            switch (_index)
            {
                case 0:
                    gameWidth = 1920;
                    gameHeight = 1080;
                    break;
                case 1:
                    gameWidth = 2560;
                    gameHeight = 1440;
                    break;
                case 2:
                    gameWidth = 1366;
                    gameHeight = 768;
                    break;
                case 3:
                    gameWidth = 1280;
                    gameHeight = 720;
                    break;
            }
            gameResolutionDropDownValue = _index;
            UpdateSettings();
        }

        public void ClientResolutionDropDown(int _index)
        {
            switch (_index)
            {
                case 0:
                    clientWidth = 1600;
                    clientHeight = 900;
                    break;
                case 1:
                    clientWidth = 1280;
                    clientHeight = 720;
                    break;
                case 2:
                    clientWidth = 1024;
                    clientHeight = 576;
                    break;
            }
            clientResolutionDropDownValue = _index;
            UpdateSettings();
        }

        public void SoundAndMusic()
        {
            music = (float)Math.Round(musicSlider.value);
            sound = (float)Math.Round(soundSlider.value);
            UpdateSettings();
        }

        private void UpdateSettings()
        {
            // Set Client Resolution
            windowScript.clientHeight = clientHeight;
            windowScript.clientWidth = clientWidth;
            StartCoroutine(windowScript.ChangeClientResolution());

            // Set Game Resolution
            windowScript.gameHeight = gameHeight;
            windowScript.gameWidth = gameWidth;

            // Handle Sound

            Debug.Log("Save");
            SaveSettings();
        }

        private void OnApplicationQuit()
        {
            Debug.Log("Save");
            SaveSettings();
        }

        public void SaveSettings()
        {
            BinaryFormatter formatter = new();
            string path = Application.persistentDataPath + "/settings.tryhard";
            FileStream stream = new(path, FileMode.Create);

            SettingsData settings = new(this);

            formatter.Serialize(stream, settings);
            stream.Close();
        }

        public SettingsData LoadSettings()
        {
            string path = Application.persistentDataPath + "/settings.tryhard";
            if (File.Exists(path))
            {
                Debug.Log("FOund");
                BinaryFormatter formatter = new();
                FileStream stream = new(path, FileMode.Open);

                SettingsData settings = (SettingsData)formatter.Deserialize(stream);
                stream.Close();

                return settings;
            }
            else
            {
                Debug.Log("Not found");
                gameWidth = defaultgameWidth;
                gameHeight = defaultgameHeight;
                clientWidth = defaultclientWidth;
                clientHeight = defaultclientHeight;
                music = defaultmusic;
                musicSlider.value = defaultmusic;
                sound = defaultsound;
                soundSlider.value = defaultsound;
                clientResolutionDropDownValue = defaultclientResolutionDropDownValue;
                clientResolutionDropDown.value = defaultclientResolutionDropDownValue;
                gameResolutionDropDownValue = defaultgameResolutionDropDownValue;
                gameResolutionDropDown.value = defaultgameResolutionDropDownValue;

                Debug.Log("Save");
                SaveSettings();
                return null;
            }
        }
    }
}