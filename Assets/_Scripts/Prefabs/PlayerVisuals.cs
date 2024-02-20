using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

namespace Player
{
    public class PlayerVisuals : NetworkBehaviour
    {
        private MeshRenderer visuals;
        [SerializeField] private Rigidbody playerRigidbody;

        private void Awake()
        {
            visuals = gameObject.GetComponentInChildren<MeshRenderer>();
        }

        public void SetVisuals(bool _var)
        {
            visuals.enabled = _var;
        }

        public void SetSize(Vector3 _var)
        {
            transform.localScale = _var;
        }

        public void SetRigidbody(bool isKinematic, bool[] constrains, float _mass)
        {
            // Set Mass
            playerRigidbody.mass = _mass;

            // Set Object Kinematic / Gravity
            if (isKinematic == true)
            {
                playerRigidbody.isKinematic = true;
                playerRigidbody.useGravity = false;
            }
            else
            {
                playerRigidbody.isKinematic = false;
                playerRigidbody.useGravity = true;

                // Set Object Constrains
                int i = 0;
                playerRigidbody.constraints =
                    (constrains[i++] ? RigidbodyConstraints.FreezePositionX : RigidbodyConstraints.None)
                 | (constrains[i++] ? RigidbodyConstraints.FreezePositionY : RigidbodyConstraints.None)
                 | (constrains[i++] ? RigidbodyConstraints.FreezePositionZ : RigidbodyConstraints.None)
                 | (constrains[i++] ? RigidbodyConstraints.FreezeRotationX : RigidbodyConstraints.None)
                 | (constrains[i++] ? RigidbodyConstraints.FreezeRotationY : RigidbodyConstraints.None)
                 | (constrains[i++] ? RigidbodyConstraints.FreezeRotationZ : RigidbodyConstraints.None);
            }
        }
    }
}