using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LocalInputData", menuName = "Data/LocalInputData")]
public class LocalInputData : ScriptableObject
{
    public bool keyZpressed;
    public bool keyXpressed;
}