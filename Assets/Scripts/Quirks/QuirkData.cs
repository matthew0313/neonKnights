using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class QuirkData : ScriptableObject
{
    [SerializeField] LangText m_quirkName, m_quirkDescription;
    [SerializeField] Color m_quirkColor;
    [SerializeField] QuirkData[] incompatible;
    public LangText quirkName => m_quirkName;
    public LangText quirkDescription => m_quirkDescription;
    public Color quirkColor => m_quirkColor;
    public virtual bool IsAddable(WeaponItem target)
    {
        foreach(var i in incompatible)
        {
            if (target.FindQuirk((other) => other.data == i, out _)) return false;
        }
        return true;
    }
    public abstract Quirk Create();
}
public abstract class Quirk
{
    public readonly QuirkData data;
    public Quirk(QuirkData data)
    {
        this.data = data;
    }
    protected WeaponItem target { get; private set; }
    public virtual void OnQuirkAdd(WeaponItem target)
    {
        this.target = target;
    }
    public virtual void OnQuirkUpdate(WeaponItem target) { }
    public virtual void SetRandom() { }
    public abstract LangText Describe();
    public virtual void Save(DataUnit data) { }
    public virtual void Load(DataUnit data) { }
}