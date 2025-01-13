using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class StatusAliment
{
    public abstract AlimentType type { get; }
    public static StatusAliment GetAlimentByType(AlimentType type)
    {
        switch (type)
        {
            default: return null;
        }
    }
    public StatusAliment()
    {

    }
    protected HpComp target { get; private set; }
    public virtual void AddTo(HpComp target)
    {
        this.target = target;
        target.AddAliment(this);
    }
    public virtual void OnAlimentAdd()
    {

    }
    public virtual void OnAlimentUpdate()
    {

    }
    public virtual void OnAlimentRemove()
    {

    }
    bool removed = false;
    public void RemoveAliment()
    {
        if (target == null || removed) return;
        removed = true;
        target.RemoveAliment(this);
    }
    public virtual void Save(DataUnit data) { }
    public virtual void Load(DataUnit data) { }
}
public enum AlimentType
{
    Slowed = 0
}