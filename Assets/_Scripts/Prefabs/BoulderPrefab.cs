using Data;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TryhardParty
{
    public class BoulderPrefab : MonoBehaviour
    {
        [Header("Scriptable")]
        public LocalData localData;
        public Firestore firestore;

        [Header("Curve")]
        public QuadraticCurve curve;
        public Transform A;
        public Transform B;
        public Transform C;

        private float sampleTime;
        private readonly float speed = 0.8f;

        public void Start()
        {
            MoveCurve();
        }

        public void Update()
        {
            if (sampleTime <= 1f)
            {
                sampleTime += Time.deltaTime * speed;
                transform.position = curve.Evaluate(sampleTime);
                transform.forward = curve.Evaluate(sampleTime + 0.001f) - transform.position;
            }
            if (sampleTime >= 1f)
            {
                Destroy(transform.parent.gameObject);
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

            sampleTime = 0f;
        }
    }
}