using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    private Light[] lights;
    private Light[] setupLights;
    private bool night;
    private bool firstTime;

    private void Start()
    {
        lights = GameObject.Find("Lights").GetComponentsInChildren<Light>();
        setupLights = GameObject.Find("SetupLights").GetComponentsInChildren<Light>();
        night = false;
        firstTime = true;
        StopLights();
        StartCoroutine(SetupLights());
    }

    public async void StartLights()
    {
        night = true;
        foreach (Light light in lights)
        {
            await Task.Delay(Random.Range(0, 100));
            light.enabled = true;
        }
    }

    public IEnumerator SetupLights()
    {
        if (firstTime == true && night == true)
        {
            Debug.Log("Setuo");
            for (int i = 0; i < setupLights.Length; i++)
            {
                Debug.Log("Light");
                Light light = setupLights[i];
                SoundManager.Instance.PlayLightSound(i);
                light.enabled = true;
                yield return new WaitForSeconds(100);
            }
            yield return new WaitForSeconds(1000);
            SoundManager.Instance.PlayLightSound(9);
            setupLights[setupLights.Length - 1].enabled = true;
            firstTime = false;
        }
        else if (night == true)
        {
            foreach (Light light in setupLights)
            {
                light.enabled = true;
                yield return new WaitForSeconds(100);
            }
            foreach (Light light in setupLights)
            {
                light.enabled = false;
                yield return new WaitForSeconds(100);
            }
        }

        yield return new WaitUntil(() => night == true);
        StartCoroutine(SetupLights());
    }

    public async void StopLights()
    {
        night = false;
        foreach (Light light in lights)
        {
            await Task.Delay(Random.Range(0, 100));
            light.enabled = false;
        }
    }
}