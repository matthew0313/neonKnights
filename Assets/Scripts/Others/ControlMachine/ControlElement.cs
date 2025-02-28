using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class ControlElement
{
    public readonly Action onControlGain;
    public ControlElement(int priority, Action onControlGain, bool enabled = true)
    {
        m_enabled = enabled;
        m_priority = priority;
        this.onControlGain = onControlGain;
    }
    bool m_enabled;
    public bool enabled
    {
        get => m_enabled;
        set
        {
            m_enabled = value;
            onElementUpdate?.Invoke();
        }
    }
    int m_priority = 0;
    public int priority
    {
        get => m_priority;
        set
        {
            m_priority = value;
            onElementUpdate?.Invoke();
        }
    }
    public Action onElementUpdate;
}