using UnityEngine;
using System;
using System.Collections.Generic;
using JetBrains.Annotations;

[CreateAssetMenu(fileName = "Equipment Random LootGen", menuName = "Scriptables/Loot Gens/Equipment (Random)")]
public class EquipmentRandomLootGen : LootGen
{
    [Header("Equipment Generation")]
    [SerializeField] EquipmentItemData[] equipments;
    [SerializeField] QuirkGroup quirks;
    [SerializeField] int minQuirkCount = 0, maxQuirkCount = 6;
    public override IEnumerable<ItemIntPair> GetLoot()
    {
        EquipmentItem equipment = equipments[UnityEngine.Random.Range(0, equipments.Length)].Create() as EquipmentItem;

        if (quirks != null)
        {
            List<QuirkData> availableQuirks = new();

            foreach (var i in quirks.GetQuirks())
            {
                if (i.IsAddable(equipment)) availableQuirks.Add(i);
            }

            int quirkCount = UnityEngine.Random.Range(minQuirkCount, maxQuirkCount + 1);
            while (quirkCount-- > 0 && availableQuirks.Count > 0)
            {
                int index = UnityEngine.Random.Range(0, availableQuirks.Count);
                Quirk tmp = availableQuirks[index].Create();
                tmp.SetRandom();
                (equipment as IQuirkable).AddQuirk(tmp);
                availableQuirks.RemoveAt(index);
            }
        }

        yield return new() { item = equipment, count = 1 };
    }
}