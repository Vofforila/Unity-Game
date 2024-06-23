using UnityEngine;

namespace Player
{
    public class CameraManager : MonoBehaviour
    {
        private float panSpeed = 35f;
        private float panBorderThickness = 15f;
        private Vector2 xPanLimit = new(-100, 100);
        private Vector2 zPanLimit = new(-80, 80);

        private void Update()
        {
            Vector3 pos = transform.position;

            if (Input.GetKey(KeyCode.UpArrow) /*|| Input.mousePosition.y >= Screen.height - panBorderThickness*/)
            {
                pos.z += panSpeed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.DownArrow) /*|| Input.mousePosition.y <= panBorderThickness*/)
            {
                pos.z -= panSpeed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.RightArrow) /*|| Input.mousePosition.x >= Screen.width - panBorderThickness*/)
            {
                pos.x += panSpeed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.LeftArrow) /*|| Input.mousePosition.x <= panBorderThickness*/)
            {
                pos.x -= panSpeed * Time.deltaTime;
            }

            pos.x = Mathf.Clamp(pos.x, xPanLimit.x, xPanLimit.y);
            pos.z = Mathf.Clamp(pos.z, zPanLimit.x, zPanLimit.y);

            transform.position = pos;
        }
    }
}