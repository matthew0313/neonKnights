using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Hook_AutoReload", menuName = "Scriptables/Hooks/Settings/Auto Reload", order = 0)]
public class Hook_AutoReload : BoolHook
{
    public override bool value => Settings.autoReload;
    public override void AssignCallback(Action callback)
    {
        Settings.onAutoReloadChange += callback;
    }

    public override void RemoveCallback(Action callback)
    {
        Settings.onAutoReloadChange -= callback;
    }
    public override void SetValue(bool value)
    {
        Settings.autoReload = value;
    }
}