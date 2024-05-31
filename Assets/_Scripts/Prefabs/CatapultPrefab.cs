using Data;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using TryhardParty;
using UnityEngine;
using UnityEngine.Events;
using SpecialFunction;
using Database;

namespace Enemy
{
    public class CatapultPrefab : NetworkBehaviour
    {
        [Header("Scriptable")]
        [SerializeField] private LocalData localData;
        [SerializeField] private Firestore firestore;
        [SerializeField] private SpecialFunctions specialFunctions;

        [Header("Prefab")]
        [SerializeField] private NetworkPrefabRef boulderPrefab;
        [SerializeField] private Transform firepoint;

        [Networked] private NetworkBool IsFiring { get; set; }


        [Header("Animator")]
        [SerializeField] private Animator animator;
        private const string FIRING = "Firing";
        private const string IDLE = "Idle";
        private const string RELOADING = "Reloading";
        public override void Spawned()
        public void Start()
        {
            IsFiring = false;
        }

        public override void Render()
        {
            if (IsFiring == false && Object.HasStateAuthority)
            {
                IsFiring = true;
                StartCoroutine(SpawnProjectile());
            }
        }

        public IEnumerator SpawnProjectile()
        {
            animator.Play(FIRING);
            yield return new WaitForEndOfFrame();

            Runner.Spawn(boulderPrefab, firepoint.transform.position, firepoint.transform.rotation);

            yield return new WaitUntil(() => IsAnimationPlaying(animator, FIRING) == false);
            animator.Play(RELOADING);
            yield return new WaitForEndOfFrame();

            yield return new WaitUntil(() => IsAnimationPlaying(animator, RELOADING) == false);
            animator.Play(IDLE);

            IsFiring = false;
        }

        public bool IsAnimationPlaying(Animator _animator, string state)
        {
            if (_animator.GetCurrentAnimatorStateInfo(0).IsName(state) && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}