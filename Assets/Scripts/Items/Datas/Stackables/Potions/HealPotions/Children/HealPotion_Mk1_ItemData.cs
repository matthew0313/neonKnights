using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Heal Potion Mk1", menuName = "Scriptables/Items/Stackables/Potions/Heal Potion", order = 0)]
public class HealPotion_Mk1_ItemData : HealPotionItemData
{
    public override Item Create()
    {
        return new HealPotion_Mk1_Item(this);
    }
}
public class HealPotion_Mk1_Item : HealPotionItem
{
    new HealPotion_Mk1_ItemData data;
    public HealPotion_Mk1_Item(HealPotion_Mk1_ItemData data) : base(data)
    {
        this.data = data;
    }
}
