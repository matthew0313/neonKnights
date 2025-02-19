using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class QuirkGroup : ScriptableObject
{
    public abstract IEnumerable<QuirkData> GetQuirks();
}