using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Status Aliments Global Data", menuName = "Scriptables/Global Datas/Status Aliments", order = 0)]
public class StatusAlimentGlobalData : ScriptableObject
{
    static StatusAlimentGlobalData instance;
    public static StatusAlimentGlobalData Instance
    {
        get
        {
            if (instance == null) instance = Resources.Load<StatusAlimentGlobalData>("Status Aliments Global Data");
            return instance;
        }
    }
    [Header("Slowed")]
    [SerializeField] Color m_slowedColor;
    [SerializeField] StatusAlimentEffect m_slowedEffect;
    public Color slowedColor => m_slowedColor;
    public StatusAlimentEffect slowedEffect => m_slowedEffect;

    [Header("Stunned")]
    [SerializeField] Color m_stunnedColor;
    [SerializeField] StatusAlimentEffect m_stunnedEffect;
    public Color stunnedColor => m_stunnedColor;
    public StatusAlimentEffect stunnedEffect => m_stunnedEffect;
}