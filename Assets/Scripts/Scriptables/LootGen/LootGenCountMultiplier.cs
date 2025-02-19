using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "LootGen Count Multiplier", menuName = "Scriptables/Loot Gens/Count Multiplier")]
public class LootGenCountMultiplier : LootGen
{
    [SerializeField] LootGen target;
    [SerializeField] int minCountMultiplier = 1, maxCountMultiplier = 2;
    public override IEnumerable<ItemIntPair> GetLoot()
    {
        foreach (var i in target.GetLoot()) yield return new() { item = i.item, count = i.count * UnityEngine.Random.Range(minCountMultiplier, maxCountMultiplier + 1) };
    }
}