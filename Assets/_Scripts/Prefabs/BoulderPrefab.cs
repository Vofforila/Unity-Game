using Data;
using Database;
using Fusion;
using SpecialFunction;
using UnityEngine;

namespace Enemy
{
    public class BoulderPrefab : NetworkBehaviour
    {
        [Header("Scriptable")]
        [SerializeField] private LocalData localData;
        [SerializeField] private Firestore firestore;

        [Header("Curve")]
        [SerializeField] private QuadraticCurve curve;
        [SerializeField] private NetworkObject curveObj;
        [SerializeField] private Transform A;
        [SerializeField] private Transform B;
        [SerializeField] private Transform C;

        [SerializeField] private Transform RotorBone;

        [Header("Game")]
        private float speed = 0.8f;
        private float rotationSpeed;

        private float sampleTime = 1f;
        private Vector3 NewPosition;
        private Vector3 NewRotation;

        public override void Spawned()
        {
            curveObj.transform.SetParent(null);
            SoundManager.Instance.PlaySound("catapult");
            sampleTime = 1f;
            MoveCurve();
        }

        public override void FixedUpdateNetwork()
        {
            if (sampleTime <= 1f)
            {
                // Calculate Trajectory
                sampleTime += Runner.DeltaTime * speed;
                NewPosition = curve.Evaluate(sampleTime);
                NewRotation = curve.Evaluate(sampleTime + 0.001f) - transform.position;

                if (NewRotation != Vector3.zero)
                {
                    transform.position = NewPosition;
                    transform.forward = NewRotation;
                    // Rotate the boulder
                    Vector3 currentRotation = RotorBone.transform.rotation.eulerAngles;
                    currentRotation.x += rotationSpeed * Runner.DeltaTime;
                    RotorBone.transform.rotation = Quaternion.Euler(currentRotation);
                }
                if (sampleTime >= 1)
                {
                    // Reset Player rotation
                    transform.rotation = Quaternion.Euler(0f, 90f, 0f);
                    transform.position = NewPosition;

                    Runner.Despawn(curveObj);
                    Runner.Despawn(Object);
                }
            }
        }

        public void MoveCurve()
        {
            float x;
            float y;
            float z;
            x = Random.Range(-24, 24);
            y = Random.Range(11, 13);
            z = Random.Range(-24, 24);

            /*   Debug Only*/
            /*B.position = new Vector3(14, 0, 14);
            C.position = new Vector3((B.localPosition.x - 2) / 2, y, B.localPosition.z / 2);*/

            B.position = new Vector3(x, 0, z);
            C.localPosition = new Vector3((B.localPosition.x - 2) / 2, y, B.localPosition.z / 2);

            speed = Random.Range(0.2f, 1f);
            rotationSpeed = speed * 2;
            sampleTime = 0f;
        }
    }
}