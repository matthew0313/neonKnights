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
    #region volumes

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

    static float m_ambienceVolume = 1.0f;
    public static float ambienceVolume
    {
        get => m_ambienceVolume;
        set
        {
            m_ambienceVolume = value;
            onAmbienceVolumeChange?.Invoke();
        }
    }
    public static Action onAmbienceVolumeChange;
    #endregion
    #region camera
    static float m_cameraSensitivity;
    public static float cameraSensitivity
    {
        get => m_cameraSensitivity;
        set
        {
            m_cameraSensitivity = value;
            onCameraSensitivityChange?.Invoke();
        }
    }
    public static Action onCameraSensitivityChange;

    static bool m_cameraYReverse;
    public static bool cameraYReverse
    {
        get => m_cameraYReverse;
        set
        {
            m_cameraYReverse = value;
            onCameraYReverseChange?.Invoke();
        }
    }
    public static Action onCameraYReverseChange;
    #endregion
    #region control
    static bool m_autoReload;
    public static bool autoReload
    {
        get => m_autoReload;
        set
        {
            m_autoReload = value;
            onAutoReloadChange?.Invoke();
        }
    }
    public static Action onAutoReloadChange;
    #endregion

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    static void ResetToDefaults()
    {
        language = Language.Korean;
        volume = 1.0f;
        cameraSensitivity = 1.0f;
        cameraYReverse = true;
        autoReload = true;
    }
    public static void Load(SettingsSave save)
    {
        Debug.Log(save.language);
        language = save.language;
        volume = save.volume;
        ambienceVolume = save.ambienceVolume;
        cameraSensitivity = save.cameraSensitivity;
        cameraYReverse = save.cameraYReverse;
        autoReload = save.autoReload;
    }
    public static SettingsSave Save()
    {
        SettingsSave save = new();
        save.language = language;
        save.volume = volume;
        save.ambienceVolume = ambienceVolume;
        save.cameraSensitivity = cameraSensitivity;
        save.cameraYReverse = cameraYReverse;
        save.autoReload = autoReload;
        return save;
    }
}
[System.Serializable]
public class SettingsSave
{
    public Language language;
    public float volume, ambienceVolume, cameraSensitivity;
    public bool cameraYReverse, autoReload;
}