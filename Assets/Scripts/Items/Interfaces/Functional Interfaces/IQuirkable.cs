using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public interface IQuirkable
{
    List<Quirk> quirks { get; }
    public void AddQuirk(Quirk quirk)
    {
        quirks.Add(quirk);
        quirk.OnQuirkAdd(this);
    }
    public bool FindQuirk(Func<Quirk, bool> predicate, out Quirk quirk)
    {
        foreach (var i in quirks)
        {
            if (predicate.Invoke(i))
            {
                quirk = i;
                return true;
            }
        }
        quirk = null;
        return false;
    }
}