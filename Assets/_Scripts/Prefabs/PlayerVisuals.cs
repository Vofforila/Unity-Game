using Fusion;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Player
{
    public class PlayerVisuals : NetworkBehaviour
    {
        private MeshRenderer visuals;
        private List<Color> colors = new() { new(0f, 0.97f, 1f, 1f), Color.white, Color.red, new(0f, 0.46f, 1f, 1f) };

        [SerializeField] private Rigidbody playerRigidbody;

        private void Awake()
        {
            visuals = gameObject.GetComponentInChildren<MeshRenderer>();
        }

        public void SetAgent(NavMeshAgent _agent, float _agentoffset, float _speed, float _angularSpeed, float _acceleration, float _stoppingDistance, float _obstacleRadius, float _obstacleHeight)
        {
            _agent.baseOffset = _agentoffset;
            _agent.speed = _speed;
            _agent.angularSpeed = _angularSpeed;
            _agent.acceleration = _acceleration;
            _agent.stoppingDistance = _stoppingDistance;
            _agent.radius = _obstacleRadius;
            _agent.height = _obstacleHeight;
        }

        public void SetPlayer(bool _visuals, int _material, float _size, bool _isKinematic, bool[] _constrains, float _mass)
        {
            visuals.material.color = colors[_material - 1];
            SetVisuals(_visuals);
            SetSize(_size);
            if (_isKinematic == false)
                SetRigidbody(_constrains, _mass);
        }

        public void SetVisuals(bool _var)
        {
            visuals.enabled = _var;
        }

        public void SetSize(float _size)
        {
            transform.localScale = Vector3.one * _size;
        }

        public void SetRigidbody(bool[] _constrains, float _mass)
        {
            // Set Mass
            Debug.Log("Error");
            playerRigidbody.mass = _mass;

            /*  // Set Object Gravity
              Debug.Log("Error");
              playerRigidbody.useGravity = true;
              Debug.Log("Error");
              // Set Object Constrains
              int i = 0;
              playerRigidbody.constraints =
                 (_constrains[i++] ? RigidbodyConstraints.FreezePositionX : RigidbodyConstraints.None)
               | (_constrains[i++] ? RigidbodyConstraints.FreezePositionY : RigidbodyConstraints.None)
               | (_constrains[i++] ? RigidbodyConstraints.FreezePositionZ : RigidbodyConstraints.None)
               | (_constrains[i++] ? RigidbodyConstraints.FreezeRotationX : RigidbodyConstraints.None)
               | (_constrains[i++] ? RigidbodyConstraints.FreezeRotationY : RigidbodyConstraints.None)
               | (_constrains[i++] ? RigidbodyConstraints.FreezeRotationZ : RigidbodyConstraints.None);*/
        }
    }
}