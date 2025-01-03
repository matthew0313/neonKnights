using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct LangText
{
    static Language m_language;
    public static Language language
    {
        get { return m_language; }
        set
        {
            m_language = value;
            onLanguageChange?.Invoke();
        }
    }
    public static Action onLanguageChange;
    public string en, kr;
    public string text
    {
        get
        {
            switch (language)
            {
                case Language.Korean: return kr;
                default: return en;
            }
        }
    }
}
[System.Serializable]
public enum Language
{
    English,
    Korean
}