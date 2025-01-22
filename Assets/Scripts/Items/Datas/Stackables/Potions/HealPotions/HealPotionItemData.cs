using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class HealPotionItemData : PotionItemData
{
    [Header("HealPotion")]
    [SerializeField] float m_healAmount;
    public float healAmount => m_healAmount;
}
public abstract class HealPotionItem : PotionItem
{
    new HealPotionItemData data;
    public HealPotionItem(HealPotionItemData data) : base(data)
    {
        this.data = data;
    }

    protected override void OnDrink()
    {
        wielder.hpComp.GetHeal(data.healAmount);
    }
    public override IEnumerable<LangText> GetStats()
    {
        yield return new()
        {
            en = $"Heal Amount: {data.healAmount}",
            kr = $"회복량: {data.healAmount}"
        };
        foreach (var i in base.GetStats()) yield return i;
    }
}
