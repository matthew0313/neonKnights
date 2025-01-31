using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Heal Potion Mk1", menuName = "Scriptables/Items/Stackables/Potions/Heal Potions/Heal Potion Mk1", order = 0)]
public class HealPotion_Mk1_ItemData : HealPotionItemData
{
    public override Item Create()
    {
        return new HealPotion_Mk1_Item(this);
    }
}
public class HealPotion_Mk1_Item : HealPotionItem
{
    new public readonly HealPotion_Mk1_ItemData data;
    new public HealPotion_Mk1 potion => base.potion as HealPotion_Mk1;
    public HealPotion_Mk1_Item(HealPotion_Mk1_ItemData data) : base(data)
    {
        this.data = data;
    }
}
