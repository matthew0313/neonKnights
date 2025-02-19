using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Chance Multi LootGen", menuName = "Scriptables/Loot Gens/Chance Multi")]
public class ChanceMultiLootGen : LootGen
{
    [SerializeField] ChanceLootGenElement[] elements;
    public override IEnumerable<ItemIntPair> GetLoot()
    {
        foreach(var i in elements)
        {
            if(UnityEngine.Random.Range(0.0f, 100.0f) <= i.chance)
            {
                foreach (var k in i.lootGen.GetLoot()) yield return k;
            }
        }
    }
}
[System.Serializable]
public struct ChanceLootGenElement
{
    public LootGen lootGen;
    public float chance;
}