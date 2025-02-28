using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class Settings
{
    static Language m_language;
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

    public static float cameraSensitivity;
    public static bool cameraYReverse;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void ResetToDefaults()
    {
        language = Language.Korean;
        volume = 1.0f;
        cameraSensitivity = 1.0f;
        cameraYReverse = true;
    }
    public static void Load(SettingsSave save)
    {
        language = save.language;
        volume = save.volume;
        cameraSensitivity = save.cameraSensitivity;
        cameraYReverse = save.cameraYReverse;
    }
    public static SettingsSave Save()
    {
        SettingsSave save = new();
        save.language = language;
        save.volume = volume;
        save.cameraSensitivity = cameraSensitivity;
        save.cameraYReverse = cameraYReverse;
        return save;
    }
}
[System.Serializable]
public class SettingsSave
{
    public Language language;
    public float volume, cameraSensitivity;
    public bool cameraYReverse;
}