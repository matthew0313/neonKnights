using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "M107_Item", menuName = "Scriptables/Items/Unstackables/Weapons/Guns/SniperRifles/M107", order = 0)]
public class M107_ItemData : SniperRifleItemData
{
    
}
public class M107_Item : SniperRifleItem
{
    new public M107_ItemData data;
    new public M107 weapon;
    public M107_Item(M107_ItemData data) : base(data)
    {
        this.data = data;
        weapon = base.weapon as M107;
    }
}
