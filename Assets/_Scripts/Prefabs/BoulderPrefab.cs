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

        [Header("Game")]
        private float speed = 0.8f;

        private float sampleTime;
        private Vector3 NewPosition;
        private Vector3 NewRotation;

        public override void Spawned()
        {
            curveObj.transform.SetParent(null);
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

            B.position = new Vector3(x, 0, z);
            C.localPosition = new Vector3((B.localPosition.x - 2) / 2, y, B.localPosition.z / 2);

            speed = Random.Range(0.2f, 1f);
            sampleTime = 0f;
        }
    }
}