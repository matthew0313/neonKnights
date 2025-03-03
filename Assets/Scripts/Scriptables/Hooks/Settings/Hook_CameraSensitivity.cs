using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Hook_CameraSensitivity", menuName = "Scriptables/Hooks/Settings/Camera Sensitivity", order = 0)]
public class Hook_CameraSensitivity : FloatHook
{
    public override float value => Settings.cameraSensitivity;
    public override void AssignCallback(Action callback)
    {
        Settings.onCameraSensitivityChange += callback;
    }

    public override void RemoveCallback(Action callback)
    {
        Settings.onCameraSensitivityChange -= callback;
    }
    public override void SetValue(float value)
    {
        Settings.cameraSensitivity = value;
    }
}