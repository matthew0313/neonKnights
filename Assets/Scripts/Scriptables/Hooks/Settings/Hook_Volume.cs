using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Hook_Volume", menuName = "Scriptables/Hooks/Settings/Volumes/Default", order = 0)]
public class Hook_Volume : FloatHook
{
    public override float value => Settings.volume;
    public override void AssignCallback(Action callback)
    {
        Settings.onVolumeChange += callback;
    }

    public override void RemoveCallback(Action callback)
    {
        Settings.onVolumeChange -= callback;
    }
    public override void SetValue(float value)
    {
        Settings.volume = value;
    }
}