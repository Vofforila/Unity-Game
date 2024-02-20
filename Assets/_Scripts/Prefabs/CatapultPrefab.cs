using Data;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TryhardParty
{
    public class CatapultPrefab : NetworkBehaviour
    {
        [Header("Scriptable")]
        public LocalData localData;
        public Firestore firestore;
        public Transform firepoint;

        [Header("Prefab")]
        public GameObject boulderPrefab;

        private bool isFiring;

        [SerializeField]
        private Animator animator;

        /*   public override void Spawned()
           {
               finish = true;
           }

           public override void FixedUpdateNetwork()
           {
               if (Object.HasStateAuthority)
               {
                   if (finish == true)
                   {
                       StartCoroutine(SpawnProjectile());
                   }
               }
           }

           public IEnumerator SpawnProjectile()
           {
               finish = false;
               animator.enabled = true;
               Instantiate(boulderPrefab, firepoint.transform.position, Quaternion.Euler(0f, -180f, 0f));
               yield return new WaitForSeconds(0.25f);
               animator.enabled = false;
               yield return new WaitForSecondsRealtime(2f);
               finish = true;
           }
       }*/

        public void Start()
        {
            isFiring = true;
        }

        public void FixedUpdate()
        {
            if (isFiring == true)
            {
                StartCoroutine(SpawnProjectile());
            }
        }

        public IEnumerator SpawnProjectile()
        {
            isFiring = false;

            animator.Play("Firing");
            Instantiate(boulderPrefab, firepoint.transform.position, Quaternion.Euler(0f, -180f, 0f));
            yield return new WaitForSecondsRealtime(2f);
            animator.Play("Idle");
            isFiring = true;
        }
    }
}