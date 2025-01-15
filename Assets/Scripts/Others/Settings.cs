using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class Settings
{
    static Language m_language = Language.English;
    public static Language language
    {
        get => m_language;
        set
        {
            m_language = value;
            onLanguageChange?.Invoke();
        }
    }
    public static Action onLanguageChange;

    static float m_volume = 1.0f;
    public static float volume
    {
        get => m_volume;
        set
        {
            m_volume = value;
            onVolumeChange?.Invoke();
        }
    }
    public static Action onVolumeChange;

    public static float cameraSensitivity = 1.0f;
    public static bool cameraYReverse = true;
}