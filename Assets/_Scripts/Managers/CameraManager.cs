using UnityEngine;

namespace Player
{
    public class CameraManager : NetworkBehaviour
    {
        public float cameraSpeed = 20f;

        private const int zoomSpeed = 50;

        // Maximum/minimum zoom distance from the ground
        public int zoomMin = 20;
        public int zoomMax = 120;

        private const int panSpeed = 40;

        // Minimal/maximal angles for camera
        private const int panAngleMin = 45;
        private const int panAngleMax = 85;

        private Vector3 newPosition = Vector3.zero;
        private float scroll;
        private int scrollArea = 25;

        private float zoomDelta;

        public void Update()
        {
            newPosition = Vector3.zero;
            // Camera Movement
            if (Input.GetKey(KeyCode.W) || Input.mousePosition.y > Screen.height - scrollArea)
            {
                newPosition = Vector3.forward;
            }
            if (Input.GetKey(KeyCode.S) || Input.mousePosition.y < scrollArea)
            {
                newPosition = Vector3.back;
            }
            if (Input.GetKey(KeyCode.A) || Input.mousePosition.x < scrollArea)
            {
                newPosition = Vector3.left;
            }
            if (Input.GetKey(KeyCode.D) || Input.mousePosition.x >= Screen.width - scrollArea)
            {
                newPosition = Vector3.right;
            }
        }

        public void FixedUpdate()
        {
            MoveCamera();
        }

        public void MoveCamera()
        {
            transform.position += cameraSpeed * Time.deltaTime * newPosition;
        }
    }
}