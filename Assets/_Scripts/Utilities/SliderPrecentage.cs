using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Utility
{
    public class SliderPrecentage : MonoBehaviour
    {
        [SerializeField] private Slider slider;

        public void UpdateSliderPrecentage()
        {
            double var = Mathf.Round(Mathf.Clamp01(slider.value) * 100);
            gameObject.GetComponent<TMP_Text>().text = var.ToString() + " %";
        }
    }
}