using Data;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using TryhardParty;
using UnityEngine;
using UnityEngine.Events;

namespace Player
{
    public class PlayerPrefab : NetworkBehaviour
    {
        [Header("Scriptable")]
        [SerializeField] private LocalData localData;

        [Header("Player Scripts")]
        [SerializeField] private Level1PlayerScript level1PlayerScript;
        [SerializeField] private Level2PlayerScript level2PlayerScript;
        [SerializeField] private Level3PlayerScript level3PlayerScript;
        [SerializeField] private Level4PlayerScript level4PlayerScript;

        public void Awake()
        {
            level1PlayerScript.enabled = false;
            level1PlayerScript.enabled = false;
            level1PlayerScript.enabled = false;
            level1PlayerScript.enabled = false;

            if (localData.currentLvl == 1)
                level1PlayerScript.enabled = true;
            if (localData.currentLvl == 2)
                level2PlayerScript.enabled = true;
            if (localData.currentLvl == 3)
                level3PlayerScript.enabled = true;
            if (localData.currentLvl == 4)
                level4PlayerScript.enabled = true;
        }
    }
}