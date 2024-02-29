using Data;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using TryhardParty;
using UnityEngine;
using UnityEngine.Events;

namespace Proxy
{
    public class BoulderPrefab : NetworkBehaviour
    {
        [Header("Scriptable")]
        [SerializeField] private LocalData localData;
        [SerializeField] private Firestore firestore;

        [Header("Curve")]
        [SerializeField] private QuadraticCurve curve;
        [SerializeField] private GameObject curveObj;
        [SerializeField] private Transform A;
        [SerializeField] private Transform B;
        [SerializeField] private Transform C;

        private float sampleTime;
        private Vector3 NewPosition;
        private Vector3 NewRotation;
        private readonly float speed = 0.8f;

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

                // Update Trajectory
                if (NewRotation != Vector3.zero)
                {
                    transform.position = NewPosition;
                    transform.forward = NewRotation;
                }
                if (sampleTime >= 1)
                {
                    // Reset Rotation
                    transform.rotation = Quaternion.Euler(0f, 90f, 0f);
                    transform.position = NewPosition;

                    Runner.Despawn(Object.GetComponent<NetworkObject>());
                    Runner.Despawn(Object);
                    Destroy(curveObj);
                }
            }
        }

        public void MoveCurve()
        {
            // Setting the Arena size
            float x;
            float y;
            float z;
            x = Random.Range(-24, 24);
            y = Random.Range(11, 13);
            z = Random.Range(-24, 24);

            B.position = new Vector3(10, 0, 10);
            C.localPosition = new Vector3((B.localPosition.x - 2) / 2, y, B.localPosition.z / 2);

            // Moving the Projectile
            sampleTime = 0f;
        }
    }
}