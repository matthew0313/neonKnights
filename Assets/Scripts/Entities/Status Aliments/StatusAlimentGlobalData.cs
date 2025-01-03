using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Status Aliments Global Data", menuName = "Scriptables/Global Datas/Status Aliments", order = 0)]
public class StatusAlimentGlobalData : ScriptableObject
{
    public static StatusAlimentGlobalData Instance { get; private set; }
    public StatusAlimentGlobalData() => Instance = this;

    [Header("Effects")]
    [SerializeField] StatusAlimentEffect m_slowedEffect;
    public StatusAlimentEffect slowedEffect => m_slowedEffect;
}