using Data;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TryhardParty
{
    public class CatapultPrefab : MonoBehaviour
    {
        [Header("Scriptable")]
        public LocalData localData;
        public Firestore firestore;
        public Transform firepoint;

        [Header("Prefab")]
        public GameObject boulderPrefab;

        private bool finish;

        [SerializeField]
        private Animator animator;

        private void Awake()
        {
            finish = true;
        }

        public void Update()
        {
            if (finish == true)
                StartCoroutine(SpawnProjectile());
        }

        public IEnumerator SpawnProjectile()
        {
            finish = false;
            animator.enabled = true;
            GameObject.Instantiate(boulderPrefab, firepoint.transform.position, Quaternion.Euler(0f, -180f, 0f));
            yield return new WaitForSeconds(0.25f);
            animator.enabled = false;
            yield return new WaitForSecondsRealtime(2f);
            finish = true;
        }
    }
}