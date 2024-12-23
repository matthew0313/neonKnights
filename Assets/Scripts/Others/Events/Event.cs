using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//no parameter overridable event
public abstract class Event
{
    public Action action;
    List<Action<Action>> overrides = new();
    public void Invoke()
    {
        Action invoked = action;
        for (int i = 0; i < overrides.Count; i++)
        {
            invoked = () => overrides[i].Invoke(invoked);
        }
        invoked?.Invoke();
    }
    public void Override(Action<Action> overrider)
    {
        overrides.Add(overrider);
    }
}
//parameter overridable event
public class Event<T>
{
    public Action<T> action;
    List<Action<Action<T>, T>> overrides = new();
    public void Invoke(T param)
    {
        Action<T> invoked = action;
        for (int i = 0; i < overrides.Count; i++)
        {
            invoked = (param) => overrides[i].Invoke(invoked, param);
        }
        invoked?.Invoke(param);
    }
    public void Override(Action<Action<T>, T> overrider)
    {
        overrides.Add(overrider);
    }
}
//parameter overridable event with a return type
public class Event<Tin, Tout>
{
    public Func<Tin, Tout> action;
    List<Func<Func<Tin, Tout>, Tin, Tout>> overrides = new();
    public Tout Invoke(Tin param)
    {
        Func<Tin, Tout> invoked = action;
        for(int i = 0; i < overrides.Count; i++)
        {
            invoked = (param) => overrides[i].Invoke(invoked, param);
        }
        return invoked.Invoke(param);
    }
    public void Override(Func<Func<Tin, Tout>, Tin, Tout> overrider)
    {
        overrides.Add(overrider);
    }
}