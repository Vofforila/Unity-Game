using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;
using System.Runtime.InteropServices;

namespace Settings
{
    public class WindowScript : MonoBehaviour, IDragHandler
    {
        private Vector2 _deltaValue = Vector2.zero;
        private bool _maximized;

        public int gameWidth = 1920;
        public int gameHeight = 1080;
        public int clientWidth = 1600;
        public int clientHeight = 900;

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

        private void Awake()
        {
            hWnd = GetActiveWindow(); //Gets the currently active window handle for use in the user32.dll functions.
        }

        private void Start()
        {
            if (hideOnStart) ShowWindowBorders(false);
        }

        public IEnumerator ChangeGameResolution()
        {
            Screen.SetResolution(gameWidth, gameHeight, true);
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
        }

        [SerializeField] private bool hideOnStart = false;

        public void ShowWindowBorders(bool value)
        {
            if (Application.isEditor) return; //We don't want to hide the toolbar from our editor!

            int style = GetWindowLong(hWnd, GWL_STYLE).ToInt32(); //gets current style

            if (value)
            {
                SetWindowLong(hWnd, GWL_STYLE, (uint)(style | WS_CAPTION | WS_SIZEBOX)); //Adds caption and the sizebox back.
                SetWindowPos(hWnd, HWND_NOTOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW); //Make the window normal.
            }
            else
            {
                SetWindowLong(hWnd, GWL_STYLE, (uint)(style & ~(WS_CAPTION | WS_SIZEBOX))); //removes caption and the sizebox from current style.
                SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW); //Make the window render above toolbar.
            }
            ResetWindowSize();
        }

        public void ResetWindowSize()
        {
            BorderlessWindow.MoveWindowPos(Vector2Int.zero, Screen.width, Screen.height);
        }

        public void OnCloseBtnClick()
        {
            EventSystem.current.SetSelectedGameObject(null);
            Application.Quit();
        }

        public void OnMinimizeBtnClick()
        {
            EventSystem.current.SetSelectedGameObject(null);
            BorderlessWindow.MinimizeWindow();
        }

        public void OnMaximizeBtnClick()
        {
            EventSystem.current.SetSelectedGameObject(null);

            if (_maximized)
                BorderlessWindow.RestoreWindow();
            else
                BorderlessWindow.MaximizeWindow();

            _maximized = !_maximized;
        }

        public void OnDrag(PointerEventData data)
        {
            _deltaValue += data.delta;
            if (data.dragging)
            {
                BorderlessWindow.MoveWindowPos(_deltaValue, Screen.width, Screen.height);
            }
        }
    }
}