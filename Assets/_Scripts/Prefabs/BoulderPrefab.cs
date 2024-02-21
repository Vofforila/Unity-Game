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
        public QuadraticCurve curve;

        [SerializeField] private Transform A;
        [SerializeField] private Transform B;
        [SerializeField] private Transform C;

        private float sampleTime;
        private Vector3 NewPosition;
        private Vector3 NewRotation;
        private readonly float speed = 0.8f;

        public override void Spawned()
        {
            MoveCurve();
        }

        public override void FixedUpdateNetwork()
        {
            if (sampleTime <= 1f)
            {
                // Calculate Trajectory
                sampleTime += Time.deltaTime * speed;
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

            // Moving the Curve to the Target Position
            B.position = new Vector3(x, 0, z);
            C.localPosition = new Vector3((B.localPosition.x - 2) / 2, y, B.localPosition.z / 2);

            // Moving the Projectile
            sampleTime = 0f;
        }
    }
}