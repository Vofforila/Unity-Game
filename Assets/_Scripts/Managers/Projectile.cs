using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TryhardParty
{
    public class Projectile : MonoBehaviour
    {
        public GameObject curveObject;
        public QuadraticCurve curve;
        private float speed = 0.2f;

        public float sampleTime;

        private void Awake()
        {
            curveObject = GameObject.Find("Curve");
            curve = curveObject.GetComponent<QuadraticCurve>(); ;
        }

        public void Jump()
        {
            sampleTime = 0f;
        }

        public void Update()
        {
            if (sampleTime <= 1f)
            {
                Debug.Log(sampleTime);
                sampleTime += Time.deltaTime * speed;
                transform.position = curve.Evaluate(sampleTime);
                transform.forward = curve.Evaluate(sampleTime + 0.001f) - transform.position;

                if (sampleTime >= 1f)
                {
                    transform.rotation = new Quaternion(0, 0, 0, 0);
                }
            }
        }
    }
}