using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpecialFunction
{
    public class QuadraticCurveManager : MonoBehaviour
    {
        [Header("Jumps")]
        [SerializeField] private List<Transform> jumpList;

        [Header("Start")]
        public Transform startPosition;

        [Header("Curve")]
        [SerializeField] public Transform A;
        [SerializeField] public Transform B;
        [SerializeField] private Transform C;

        private readonly List<int> scoreList = new() { 200, 50, 100, 150, 200, 250, 500 };

        public static QuadraticCurveManager Instance;

        private void Awake()
        {
            Instance = this;
        }

        public int MoveCurve(int _jumpPosition)
        {
            A.position = B.position;
            B.position = jumpList[_jumpPosition].position;
            C.localPosition = new Vector3(0, 5, (A.localPosition.z + jumpList[_jumpPosition].localPosition.z) / 2);

            return scoreList[_jumpPosition];
        }
    }
}