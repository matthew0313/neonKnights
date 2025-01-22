using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AWP_Item", menuName = "Scriptables/Items/Unstackables/Weapons/Guns/SniperRifles/AWP", order = 0)]
public class AWP_ItemData : SniperRifleItemData
{
    public override Item Create()
    {
        return new AWP_Item(this);
    }
}
public class AWP_Item : SniperRifleItem
{
    new public readonly AWP_ItemData data;
    new public AWP weapon => base.weapon as AWP;
    public AWP_Item(AWP_ItemData data) : base(data)
    {
        this.data = data;
    }
}
