using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "Choice LootGen", menuName = "Scriptables/Loot Gens/Choice")]
public class ChoiceLootGen : LootGen
{
    [SerializeField] ItemData[] choices;
    [SerializeField] int minCount = 1, maxCount = 1;
    public override IEnumerable<ItemIntPair> GetLoot()
    {
        yield return new() { item = choices[UnityEngine.Random.Range(0, choices.Length)].Create(), count = UnityEngine.Random.Range(minCount, maxCount) };
    }
}