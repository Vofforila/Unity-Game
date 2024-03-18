using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Settings
{
    public class SettingManager : MonoBehaviour
    {
        public int gameWidth;
        public int gameHeight;
        public int clientWidth;
        public int clientHeight;

        private void Awake()
        {
            LoadSetting();
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
        }

        public void ClientResolutionDropDown(int _index)
        {
            switch (_index)
            {
                case 0:
                    clientWidth = 1280;
                    clientHeight = 720;
                    break;
                case 1:
                    gameWidth = 1024;
                    gameHeight = 576;
                    break;
                case 2:
                    gameWidth = 1600;
                    gameHeight = 900;
                    break;
            }
        }

        private void OnApplicationQuit()
        {
            SaveSetting();
        }

        public void SaveSetting()
        {
            BinaryFormatter formatter = new();
            string path = Application.persistentDataPath + "/settings.tryhard";
            FileStream stream = new(path, FileMode.Create);

            SettingsData settings = new(this);

            formatter.Serialize(stream, settings);
            stream.Close();
        }

        public SettingsData LoadSetting()
        {
            string path = Application.persistentDataPath + "/settings.tryhard";
            if (File.Exists(path))
            {
                BinaryFormatter formatter = new();
                FileStream stream = new(path, FileMode.Open);

                SettingsData settings = (SettingsData)formatter.Deserialize(stream);
                stream.Close();

                return settings;
            }
            else
            {
                SaveSetting();
                return null;
            }
        }
    }
}