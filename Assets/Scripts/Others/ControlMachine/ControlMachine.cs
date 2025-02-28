using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlMachine
{
    readonly Action defaultControl;
    public ControlMachine(Action defaultControl = null)
    {
        this.defaultControl = defaultControl;
    }

    List<ControlElement> controls = new();
    void UpdateControlList()
    {
        controls.Sort((a, b) =>
        {
            if (!a.enabled)
            {
                if (!b.enabled) return 0;
                else return 1;
            }
            if (!b.enabled) return -1;

            return b.priority.CompareTo(a.priority);
        });
        Debug.Log("Updated");
        if (controls.Count > 0 && controls[0].enabled) controls[0].onControlGain.Invoke();
        else defaultControl?.Invoke();
    }
    public void AddControls(params ControlElement[] controls)
    {
        foreach(var i in controls)
        {
            this.controls.Add(i);
            i.onElementUpdate += UpdateControlList;
        }
        UpdateControlList();
    }
    public void RemoveControl(ControlElement control)
    {
        controls.Remove(control);
        control.onElementUpdate -= UpdateControlList;

        UpdateControlList();
    }
}