using UnityEngine;
using System;
using System.Collections.Generic;
using JetBrains.Annotations;

[CreateAssetMenu(fileName = "Weapon Set LootGen", menuName = "Scriptables/Loot Gens/Weapon (Set)")]
public class WeaponSetLootGen : LootGen
{
    [Header("Weapon Generation")]
    [SerializeField] WeaponItemData weapon;
    [SerializeField] QuirkData[] quirks;
    public override IEnumerable<ItemIntPair> GetLoot()
    {
        WeaponItem weapon = this.weapon.Create() as WeaponItem;
        foreach(var i in quirks)
        {
            if (i.IsAddable(weapon))
            {
                Quirk tmp = i.Create();
                tmp.SetRandom();
                (weapon as IQuirkable).AddQuirk(tmp);
            }
        }
        yield return new() { item = weapon, count = 1 };
    }
}