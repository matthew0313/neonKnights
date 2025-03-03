using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Hook_CameraYReverse", menuName = "Scriptables/Hooks/Settings/Camera Y Reverse", order = 0)]
public class Hook_CameraYReverse : BoolHook
{
    public override bool value => Settings.cameraYReverse;
    public override void AssignCallback(Action callback)
    {
        Settings.onCameraYReverseChange += callback;
    }

    public override void RemoveCallback(Action callback)
    {
        Settings.onCameraYReverseChange -= callback;
    }
    public override void SetValue(bool value)
    {
        Settings.cameraYReverse = value;
    }
}