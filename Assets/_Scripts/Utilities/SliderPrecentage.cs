using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

namespace Utility
{
    public class SliderPrecentage : MonoBehaviour
    {
        [SerializeField] private Slider slider;

        public void UpdateSliderPrecentage()
        {
            double var = Math.Round(slider.value);
            gameObject.GetComponent<TMP_Text>().text = var.ToString() + " %";
        }
    }
}