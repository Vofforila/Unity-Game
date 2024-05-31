using Fusion;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

namespace SpecialFunction
{
    [CreateAssetMenu(fileName = "SpecialFunctions", menuName = "Utilities/SpecialFunctions")]
    public class SpecialFunctions : ScriptableObject
    {
        /// <summary>
        /// Takes a String and makes it uppercase
        ///
        /// <para> <c>Returns:</c> String </para>
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string UpperCase(string input)
        {
            // Create a TextInfo object for the current culture
            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;

            // Use ToTitleCase method to capitalize the first letter of each word
            return textInfo.ToTitleCase(input);
        }

        /// <summary>
        /// Destorys all Childrens of a Object
        /// </summary>
        /// <param name="_gameobject"></param>
        public Task DestroyChildrenOf(GameObject _gameobject)
        {
            foreach (Transform child in _gameobject.transform)
            {
                Destroy(child.gameObject);
            }
            return Task.CompletedTask;
        }

        /// <summary>
        ///  Takes a FireStore Dictionary and Returns a FirestoreObject
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public static T CreateObjectFromDictionary<T>(Dictionary<string, object> dictionary) where T : new()
        {
            T obj = new T();
            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                if (dictionary.ContainsKey(property.Name))
                {
                    object value = dictionary[property.Name];

                    // Convert value if necessary
                    if (property.PropertyType != value.GetType())
                    {
                        value = Convert.ChangeType(value, property.PropertyType);
                    }

                    property.SetValue(obj, value);
                }
            }

            return obj;
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