using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "LootGen Attempter", menuName = "Scriptables/Loot Gens/Attempter")]
public class LootGenAttempter : LootGen
{
    [SerializeField] LootGen target;
    [SerializeField] int minAttempt = 1, maxAttempt = 2;
    public override IEnumerable<ItemIntPair> GetLoot()
    {
        int attempt = UnityEngine.Random.Range(minAttempt, maxAttempt + 1);
        for(int i = 0; i < attempt; i++)
        {
            foreach (var k in target.GetLoot()) yield return k;
        }
    }
}