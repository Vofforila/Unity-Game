using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.UI;
using System;
using TMPro;
using Server;
using UI;
using System.Collections;
using System.Runtime.InteropServices;

namespace Settings
{
    public class SettingManager : MonoBehaviour
    {
        #region DLLstuff

        private const int SWP_HIDEWINDOW = 0x80; //hide window flag.
        private const int SWP_SHOWWINDOW = 0x40; //show window flag.
        private const int SWP_NOMOVE = 0x0002; //don't move the window flag.
        private const int SWP_NOSIZE = 0x0001; //don't resize the window flag.
        private const uint WS_SIZEBOX = 0x00040000;
        private const int GWL_STYLE = -16;
        private const int WS_BORDER = 0x00800000; //window with border
        private const int WS_DLGFRAME = 0x00400000; //window with double border but no title
        private const int WS_CAPTION = WS_BORDER | WS_DLGFRAME; //window with a title bar
        private const int WS_SYSMENU = 0x00080000;      //window with no borders etc.
        private const int WS_MAXIMIZEBOX = 0x00010000;
        private const int WS_MINIMIZEBOX = 0x00020000;  //window with minimizebox

        [DllImport("user32.dll")]
        private static extern System.IntPtr GetActiveWindow();

        [DllImport("user32.dll")]
        private static extern int FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(
            System.IntPtr hWnd, // window handle
            System.IntPtr hWndInsertAfter, // placement order of the window
            short X, // x position
            short Y, // y position
            short cx, // width
            short cy, // height
            uint uFlags // window flags.
        );

        [DllImport("user32.dll")]
        private static extern System.IntPtr SetWindowLong(
             System.IntPtr hWnd, // window handle
             int nIndex,
             uint dwNewLong
        );

        [DllImport("user32.dll")]
        private static extern System.IntPtr GetWindowLong(
            System.IntPtr hWnd,
            int nIndex
        );

        private System.IntPtr hWnd;
        private System.IntPtr HWND_TOP = new System.IntPtr(0);
        private System.IntPtr HWND_TOPMOST = new System.IntPtr(-1);
        private System.IntPtr HWND_NOTOPMOST = new System.IntPtr(-2);

        #endregion DLLstuff

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
        public short clientWidth;
        public short clientHeight;
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

        public static SettingManager Instance;

        private void Awake()
        {
            Instance = this;
            hWnd = GetActiveWindow(); //Gets the currently active window handle for use in the user32.dll functions.
        }

        private void Start()
        {
            SettingsData settings = LoadSettings();

            // Make Unity Borderless
            ShowWindowBorders(false);

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
            StartCoroutine(IChangeResolution());

            // Handle Sound
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
                BinaryFormatter formatter = new();
                FileStream stream = new(path, FileMode.Open);

                SettingsData settings = (SettingsData)formatter.Deserialize(stream);
                stream.Close();

                return settings;
            }
            else
            {
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

        private const uint WS_POPUP = 0x80000000;
        private const uint WS_VISIBLE = 0x10000000;

        public void ShowWindowBorders(bool value)
        {
            if (Application.isEditor) return; //We don't want to hide the toolbar from our editor!

            int style = GetWindowLong(hWnd, GWL_STYLE).ToInt32(); //gets current style

            if (value)
            {
                SetWindowLong(hWnd, GWL_STYLE, (uint)(style | WS_CAPTION | WS_SIZEBOX)); //Adds caption and the sizebox back.
                SetWindowPos(hWnd, HWND_NOTOPMOST, 0, 0, clientWidth, clientHeight, SWP_SHOWWINDOW); //Make the window normal.
            }
            else
            {
                SetWindowLong(hWnd, GWL_STYLE, WS_POPUP | WS_VISIBLE /*(uint)(style & ~(WS_CAPTION | WS_SIZEBOX))*/); //removes caption and the sizebox from current style.
            }
            ResetWindowSize();
        }

        public void ResetWindowSize()
        {
            Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
            BorderlessWindow.MoveWindowPos(Vector2Int.zero, Screen.width, Screen.height);
        }

        public IEnumerator IChangeResolution()
        {
            Screen.SetResolution(clientWidth, clientHeight, FullScreenMode.Windowed);
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            ShowWindowBorders(false);
        }
    }
}