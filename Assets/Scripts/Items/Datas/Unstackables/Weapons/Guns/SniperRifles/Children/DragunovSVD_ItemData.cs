using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DragunovSVD_Item", menuName = "Scriptables/Items/Unstackables/Weapons/Guns/SniperRifles/DragunovSVD", order = 0)]
public class DragunovSVD_ItemData : SniperRifleItemData
{
    public override Item Create()
    {
        return new DragunovSVD_Item(this);
    }
}
public class DragunovSVD_Item : SniperRifleItem
{
    new public readonly DragunovSVD_ItemData data;
    new public DragunovSVD weapon => base.weapon as DragunovSVD;
    public DragunovSVD_Item(DragunovSVD_ItemData data) : base(data)
    {
        this.data = data;
    }
}