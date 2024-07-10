using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    public Slider musicVolume;
    public Slider SFXVolume;
    public AudioSource musicSource;
    AudioSource SFXSource;
    public GameObject player;

    void Start()
    {
        SFXSource = player.GetComponent<AudioSource>();

        musicVolume.value = musicSource.volume;
        musicVolume.onValueChanged.AddListener(OnMusicVolumeChange);

        SFXVolume.value = SFXSource.volume;
        SFXVolume.onValueChanged.AddListener(OnSFXVolumeChange);
    }

    void OnMusicVolumeChange(float value)
    {
        musicSource.volume = value;
    }

    void OnSFXVolumeChange(float value)
    {
        SFXSource.volume = value;
    }
}
