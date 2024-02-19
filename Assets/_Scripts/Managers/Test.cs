using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace TryhardParty
{
    public class Test : MonoBehaviour
    {
        public void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Finish"))
            {
                Debug.Log("Collision");
            }
        }
    }
}