using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(AudioSource))]
public class AudioSourceVolumer : MonoBehaviour
{
    [SerializeField] FloatHook[] volumeHook;
    [Range(0.0f, 1.0f)][SerializeField] float multiplier = 1.0f;
    AudioSource source;
    private void OnEnable()
    {
        if(source == null) source = GetComponent<AudioSource>();

        Settings.onVolumeChange += OnVolumeChange;
        foreach (var i in volumeHook)
        {
            i.AssignCallback(OnVolumeChange);
        }
        OnVolumeChange();
    }
    private void OnDisable()
    {
        Settings.onVolumeChange -= OnVolumeChange;
        foreach (var i in volumeHook)
        {
            i.RemoveCallback(OnVolumeChange);
        }
    }
    void OnVolumeChange()
    {
        float tmp = Settings.volume * multiplier;
        foreach (var i in volumeHook)
        {
            tmp *= i.value;
        }
        source.volume = Mathf.Clamp01(tmp);
    }
}