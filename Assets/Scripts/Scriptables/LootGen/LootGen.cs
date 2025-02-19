using UnityEngine;
using System;
using System.Collections.Generic;

public abstract class LootGen : ScriptableObject
{
    public abstract IEnumerable<ItemIntPair> GetLoot();
}
