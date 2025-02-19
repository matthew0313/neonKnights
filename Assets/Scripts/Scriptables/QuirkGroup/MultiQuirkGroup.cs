using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Multi Quirk Group", menuName = "Scriptables/Quirk Groups/Multi")]
public class MultiQuirkGroup : QuirkGroup
{
    [SerializeField] QuirkGroup[] quirkGroups;
    public override IEnumerable<QuirkData> GetQuirks()
    {
        foreach(var i in quirkGroups)
        {
            foreach (var k in i.GetQuirks()) yield return k;
        }
    }
}