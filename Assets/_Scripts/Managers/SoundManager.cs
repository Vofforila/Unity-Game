using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("Sound")]
    [SerializeField] private List<Sound> sounds;
    [SerializeField] private List<Sound> musics;

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

        foreach (Sound music in musics)
        {
            music.source = gameObject.AddComponent<AudioSource>();
            music.source.clip = music.clip;
            music.source.mute = music.mute;
            music.source.bypassEffects = music.bypassEffect;
            music.source.bypassListenerEffects = music.bypassListenerEffects;
            music.source.bypassReverbZones = music.bypassReverbZones;
            music.source.loop = music.loop;
            music.source.volume = music.volume;
            music.source.pitch = music.pitch;
            music.source.spatialBlend = music.spatialBlend;
        }

        DontDestroyOnLoad(gameObject);
    }

    public void Start()
    {
        PlayMusic("music-1");
    }

    public void ChangeMusicVolume(float _volume)
    {
        foreach (Sound music in musics)
        {
            music.source.volume = _volume;
        }
    }

    public void ChangeSoundVolume(float _volume)
    {
        foreach (Sound sound in sounds)
        {
            sound.source.volume = _volume;
        }
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

    public void PlayMusic(string _name)
    {
        foreach (Sound music in musics)
        {
            if (music.name == _name)
            {
                music.source.Play();
            }
        }
    }

    public void StopMusic(string _name)
    {
        foreach (Sound music in musics)
        {
            if (music.name == _name)
            {
                music.source.Stop();
            }
        }
    }

    public void DestoyYourself()
    {
        Destroy(gameObject);
    }
}