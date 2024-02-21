using Data;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using TryhardParty;
using UnityEngine;
using UnityEngine.Events;

namespace Proxy
{
    public class CatapultPrefab : NetworkBehaviour
    {
        [Header("Scriptable")]
        [SerializeField] private LocalData localData;

        [SerializeField] private Firestore firestore;
        [SerializeField] private Transform firepoint;

        [Header("Prefab")]
        [SerializeField] private NetworkPrefabRef boulderPrefab;

        [Networked] private bool IsFiring { get; set; }

        [Networked] private bool FinishedFiring { get; set; }

        [Header("Animator")]
        [SerializeField] private Animator animator;

        public void Start()
        {
            IsFiring = true;
        }

        public override void FixedUpdateNetwork()
        {
            FinishedFiring = animator.GetCurrentAnimatorStateInfo(0).IsName("Firing");

            if (IsFiring == true)
            {
                StartCoroutine(SpawnProjectile());
            }
        }

        public IEnumerator SpawnProjectile()
        {
            IsFiring = false;
            // Test the animations

            animator.SetBool("IsFiring", true);

            Runner.Spawn(boulderPrefab, firepoint.transform.position, firepoint.transform.rotation);

            yield return new WaitUntil(() => FinishedFiring == true);

            animator.SetBool("IsFiring", false);
            yield return new WaitForSecondsRealtime(5f);

            IsFiring = true;
        }
    }
}