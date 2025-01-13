using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct LangText
{
    public string en, kr;
    public string text
    {
        get
        {
            switch (Settings.language)
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