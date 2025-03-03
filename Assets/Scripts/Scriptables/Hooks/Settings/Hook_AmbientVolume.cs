using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Hook_AmbientVolume", menuName = "Scriptables/Hooks/Settings/Volumes/Ambient", order = 0)]
public class Hook_AmbientVolume : FloatHook
{
    public override float value => Settings.ambienceVolume;
    public override void AssignCallback(Action callback)
    {
        Settings.onAmbienceVolumeChange += callback;
    }

    public override void RemoveCallback(Action callback)
    {
        Settings.onAmbienceVolumeChange -= callback;
    }

    public override void SetValue(float value)
    {
        Settings.ambienceVolume = value;
    }
}