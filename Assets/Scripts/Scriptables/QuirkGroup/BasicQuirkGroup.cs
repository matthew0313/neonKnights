using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Basic Quirk Group", menuName = "Scriptables/Quirk Groups/Basic")]
public class BasicQuirkGroup : QuirkGroup
{
    [SerializeField] QuirkData[] quirks;
    public override IEnumerable<QuirkData> GetQuirks()
    {
        foreach(var i in quirks)
        {
            yield return i;
        }
    }
}