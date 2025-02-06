using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public abstract class QuirkData : ScriptableObject
{
    [SerializeField] LangText m_quirkName, m_quirkDescription;
    [SerializeField][ColorUsage(false)] Color m_quirkColor;
    [SerializeField] Sprite m_quirkIcon;
    [SerializeField] QuirkData[] incompatible;
    public LangText quirkName => m_quirkName;
    public LangText quirkDescription => m_quirkDescription;
    public Color quirkColor => m_quirkColor;
    public Sprite quirkIcon => m_quirkIcon;
    const int maxQuirks = 6;

    public virtual bool IsAddable(IQuirkable target)
    {
        if (target.quirks.Count >= maxQuirks) return false;
        if(target.FindQuirk((other) => other.data == this, out _)) return false;
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
    protected IQuirkable target { get; private set; }
    public virtual void OnQuirkAdd(IQuirkable target)
    {
        this.target = target;
    }
    public virtual void SetRandom() { }
    public virtual void Save(DataUnit data) { }
    public virtual void Load(DataUnit data) { }
    public abstract LangText Describe();
    public virtual Quirk Copy()
    {
        return data.Create();
    }

    internal void OnQuirkRemove(IQuirkable target)
    {
        throw new NotImplementedException();
    }
}