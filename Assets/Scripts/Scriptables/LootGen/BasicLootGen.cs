using UnityEngine;
using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "Chance LootGen", menuName = "Scriptables/Loot Gens/Basic")]
public class BasicLootGen : LootGen
{
    [SerializeField] BasicLootGenElement[] elements;
    public override IEnumerable<ItemIntPair> GetLoot()
    {
        foreach(var i in elements)
        {
            if(UnityEngine.Random.Range(0.0f, 100.0f) <= i.chance)
            {
                yield return new() { item = i.item.Create(), count = UnityEngine.Random.Range(i.minCount, i.maxCount + 1) };
            }
        }
    }
}
[System.Serializable]
public struct BasicLootGenElement
{
    public ItemData item;
    public float chance;
    public int minCount, maxCount;
}