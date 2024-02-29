using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using Fusion;

namespace SpecialFunction
{
    [CreateAssetMenu(fileName = "SpecialFunctions", menuName = "Utilities/SpecialFunctions")]
    public class SpecialFunctions : ScriptableObject
    {
        public string UpperCase(string input)
        {
            // Create a TextInfo object for the current culture
            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;

            // Use ToTitleCase method to capitalize the first letter of each word
            return textInfo.ToTitleCase(input);
        }

        public void DestroyChildrenOf(GameObject _gameobject)
        {
            foreach (Transform child in _gameobject.transform)
            {
                Destroy(child.gameObject);
            }
        }

        public Dictionary<PlayerRef, GameObject> InstanciateChildrenFor(GameObject _gameobject, Transform _parentTransform, IEnumerable<PlayerRef> _players)
        {
            Dictionary<PlayerRef, GameObject> playerScoreDictionary = new();
            foreach (PlayerRef player in _players)
            {
                GameObject instanciatedObject = Instantiate(_gameobject, _parentTransform);
                playerScoreDictionary.Add(player, instanciatedObject);
            }
            return playerScoreDictionary;
        }
    }
}