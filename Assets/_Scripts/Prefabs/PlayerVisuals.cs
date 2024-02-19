using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

namespace Player
{
    public class PlayerVisuals : NetworkBehaviour
    {
        private MeshRenderer visuals;

        private void Awake()
        {
            visuals = gameObject.GetComponentInChildren<MeshRenderer>();
        }

        public void SetVisuals(bool _var)
        {
            Debug.Log(_var);
            visuals.enabled = _var;
        }

        public void SetSize(Vector3 _var)
        {
            transform.localScale = _var;
        }
    }
}