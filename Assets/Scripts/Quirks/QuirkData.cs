using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public abstract class QuirkData : ScriptableObject
{
    [SerializeField][ColorUsage(false)] Color m_quirkColor;
    [SerializeField] Sprite m_quirkIcon;
    [SerializeField] QuirkData[] incompatible;
    public Color quirkColor => m_quirkColor;
    public Sprite quirkIcon => m_quirkIcon;
    public virtual bool IsAddable(IQuirkable target)
    {
        foreach(var i in incompatible)
        {
            if (target.FindQuirk((other) => other.data == i || other.data == this, out _)) return false;
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
    public abstract LangText quirkName { get; }
    public abstract LangText quirkDescription { get; }
    public abstract LangText Describe();
    public virtual Quirk Copy()
    {
        return data.Create();
    }
}