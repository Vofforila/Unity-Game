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

        public void SetPlayer(bool _visuals, Vector3 _size, bool _isKinematic, bool[] _constrains, float _mass)
        {
            SetVisuals(_visuals);
            SetSize(_size);
            if (_isKinematic == false)
                SetRigidbody(_constrains, _mass);
        }

        public void SetVisuals(bool _var)
        {
            visuals.enabled = _var;
        }

        public void SetSize(Vector3 _var)
        {
            transform.localScale = _var;
        }

        public void SetRigidbody(bool[] _constrains, float _mass)
        {
            // Set Mass
            playerRigidbody.mass = _mass;

            // Set Object Gravity

            playerRigidbody.useGravity = true;

            // Set Object Constrains
            int i = 0;
            playerRigidbody.constraints =
               (_constrains[i++] ? RigidbodyConstraints.FreezePositionX : RigidbodyConstraints.None)
             | (_constrains[i++] ? RigidbodyConstraints.FreezePositionY : RigidbodyConstraints.None)
             | (_constrains[i++] ? RigidbodyConstraints.FreezePositionZ : RigidbodyConstraints.None)
             | (_constrains[i++] ? RigidbodyConstraints.FreezeRotationX : RigidbodyConstraints.None)
             | (_constrains[i++] ? RigidbodyConstraints.FreezeRotationY : RigidbodyConstraints.None)
             | (_constrains[i++] ? RigidbodyConstraints.FreezeRotationZ : RigidbodyConstraints.None);
        }
    }
}