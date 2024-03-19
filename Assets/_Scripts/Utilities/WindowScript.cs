using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

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

        public IEnumerator ChangeGameResolution()
        {
            Screen.SetResolution(gameWidth, gameHeight, true);
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
        }

        public IEnumerator ChangeClientResolution()
        {
            Screen.SetResolution(clientWidth, clientHeight, false);
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
#if !UNITY_EDITOR && UNITY_STANDALONE_WIN   // Dont do this while on Unity Editor!
            BorderlessWindow.SetFramelessWindow();
#endif
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