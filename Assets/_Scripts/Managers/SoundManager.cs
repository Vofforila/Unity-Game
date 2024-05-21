using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("Sound")]
    [SerializeField] private List<Sound> sounds;

    public static SoundManager Instance;

    private void Awake()
    {
        Instance = this;

        foreach (Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.mute = sound.mute;
            sound.source.bypassEffects = sound.bypassEffect;
            sound.source.bypassListenerEffects = sound.bypassListenerEffects;
            sound.source.bypassReverbZones = sound.bypassReverbZones;
            sound.source.loop = sound.loop;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.spatialBlend = sound.spatialBlend;
        }

        DontDestroyOnLoad(gameObject);
    }

    public void Start()
    {
        PlaySound("LightSound");
    }

    public void StopSound(string _name)
    {
        foreach (Sound sound in sounds)
        {
            if (sound.name == _name)
            {
                sound.source.Stop();
            }
        }
    }

    public void PlaySound(string _name)
    {
        foreach (Sound sound in sounds)
        {
            if (sound.name == _name)
            {
                sound.source.Play();
            }
        }
    }

    public void DestoryYourself()
    {
        Destroy(gameObject);
    }
}