using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "M24_Item", menuName = "Scriptables/Items/Unstackables/Weapons/Guns/SniperRifles/M24", order = 0)]
public class M24_ItemData : SniperRifleItemData
{
    
}
public class M24_Item : SniperRifleItem
{
    new public M24_ItemData data;
    new public M24 weapon;
    public M24_Item(M24_ItemData data) : base(data)
    {
        this.data = data;
        weapon = base.weapon as M24;
    }
}