using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("Music")]
    [SerializeField] private AudioClip music;

    [Header("Sound Effects")]
    [SerializeField] private List<AudioClip> sound;
    [SerializeField] private List<AudioClip> lightOpens;

    [Header("Sound Sources")]
    [SerializeField] private List<AudioSource> soundSourcer;

    public static SoundManager Instance;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);
    }

    /*private void Start()
    {
        soundSourcer[0].clip = music;
    }*/

    public void PlayLightSound(int _var)
    {
        soundSourcer[1].clip = lightOpens[_var];
    }
}