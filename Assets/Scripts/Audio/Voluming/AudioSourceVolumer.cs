using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioSourceVolumer : MonoBehaviour
{
    [SerializeField] FloatHook[] volumeHook;
    [Range(0.0f, 1.0f)][SerializeField] float multiplier = 1.0f;
    AudioSource source;
    private void OnEnable()
    {
        if(source == null) source = GetComponent<AudioSource>();

        if(volumeHook.Length == 0)
        {
            Settings.onVolumeChange += OnVolumeChange;
        }
        else
        {
            foreach(var i in volumeHook)
            {
                i.AssignCallback(OnVolumeChange);
            }
        }
        OnVolumeChange();
    }
    private void OnDisable()
    {
        if(volumeHook.Length == 0)
        {
            Settings.onVolumeChange -= OnVolumeChange;
        }
        else
        {
            foreach(var i in volumeHook)
            {
                i.RemoveCallback(OnVolumeChange);
            }
        }
    }
    void OnVolumeChange()
    {
        if (volumeHook.Length == 0)
        {
            source.volume = Mathf.Clamp01(Settings.volume * multiplier);
        }
        else
        {
            float volume = 1.0f;
            foreach(var i in volumeHook)
            {
                volume *= i.value;
            }
            source.volume = Mathf.Clamp01(volume * multiplier);
        }
    }
}