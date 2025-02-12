using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioSourceVolumer : MonoBehaviour
{
    [SerializeField] float multiplier = 1.0f;
    AudioSource source;
    private void OnEnable()
    {
        if(source == null) source = GetComponent<AudioSource>();
        Settings.onVolumeChange += OnVolumeChange;
        OnVolumeChange();
    }
    private void OnDisable()
    {
        Settings.onVolumeChange -= OnVolumeChange;
    }
    void OnVolumeChange()
    {
        source.volume = Settings.volume * multiplier;
    }
}