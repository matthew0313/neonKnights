using UnityEngine;
using System;
using System.Collections.Generic;
using JetBrains.Annotations;

[CreateAssetMenu(fileName = "Weapon LootGen", menuName = "Scriptables/Loot Gens/Weapon")]
public class WeaponLootGen : LootGen
{
    [Header("Weapon Generation")]
    [SerializeField] WeaponItemData[] weapons;
    [SerializeField] QuirkGroup quirks;
    [SerializeField] int minQuirkCount = 0, maxQuirkCount = 6;
    public override IEnumerable<ItemIntPair> GetLoot()
    {
        WeaponItem weapon = weapons[UnityEngine.Random.Range(0, weapons.Length)].Create() as WeaponItem;

        if (quirks != null)
        {
            List<QuirkData> availableQuirks = new();

            foreach (var i in quirks.GetQuirks())
            {
                if (i.IsAddable(weapon)) availableQuirks.Add(i);
            }

            int quirkCount = UnityEngine.Random.Range(minQuirkCount, maxQuirkCount + 1);
            while (quirkCount-- > 0 && availableQuirks.Count > 0)
            {
                int index = UnityEngine.Random.Range(0, availableQuirks.Count);
                Quirk tmp = availableQuirks[index].Create();
                tmp.SetRandom();
                (weapon as IQuirkable).AddQuirk(tmp);
                availableQuirks.RemoveAt(index);
            }
        }

        yield return new() { item = weapon, count = 1 };
    }
}