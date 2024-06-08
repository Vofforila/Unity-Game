using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    private Light[] lights;
    private Light[] setupLights;
    private bool night;
    private bool firstTime;

    public static LightManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        lights = GameObject.Find("Lights").GetComponentsInChildren<Light>();
        setupLights = GameObject.Find("SetupLights").GetComponentsInChildren<Light>();
        night = false;
        firstTime = true;
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

    public async void StopLights()
    {
        night = false;
        foreach (Light light in lights)
        {
            await Task.Delay(Random.Range(0, 100));
            light.enabled = false;
        }
    }

    public IEnumerator SetupLights()
    {
        while (true)
        {
            if (firstTime == true)
            {
                for (int i = 0; i < setupLights.Length - 1; i++)
                {
                    Light light = setupLights[i];
                    SoundManager.Instance.PlaySound("light-swich-" + i);
                    light.enabled = true;
                    yield return new WaitForSeconds(0.5f);
                }
                yield return new WaitForSeconds(1f);
                SoundManager.Instance.PlaySound("light-swich-9");
                setupLights[^1].enabled = true;
                firstTime = false;
                yield return new WaitForSeconds(10f);
            }
            else if (night == true)
            {
                for (int i = 0; i < setupLights.Length - 2; i++)
                {
                    Light light = setupLights[i];
                    light.enabled = true;
                    yield return new WaitForSeconds(0.1f);
                }
            }
            for (int i = 0; i < setupLights.Length - 2; i++)
            {
                Light light = setupLights[i];
                light.enabled = false;
                yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitUntil(() => night == true);
        }
    }
}