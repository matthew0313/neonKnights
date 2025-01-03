using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatusAliment
{
    public StatusAliment()
    {

    }
    protected HpComp target { get; private set; }
    public virtual void OnAlimentAdd(HpComp target)
    {
        this.target = target;
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
    public static StatusAliment GetAlimentByType(AlimentType type)
    {
        switch (type)
        {
            default: return null;
        }
    }
}
public enum AlimentType
{
    Slowed = 0
}