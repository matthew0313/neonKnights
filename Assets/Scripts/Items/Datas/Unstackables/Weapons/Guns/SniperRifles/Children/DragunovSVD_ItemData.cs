using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DragunovSVD_Item", menuName = "Scriptables/Items/Unstackables/Weapons/Guns/SniperRifles/DragunovSVD", order = 0)]
public class DragunovSVD_ItemData : SniperRifleItemData
{
    
}
public class DragunovSVD_Item : SniperRifleItem
{
    new public DragunovSVD_ItemData data;
    new public DragunovSVD weapon;
    public DragunovSVD_Item(DragunovSVD_ItemData data) : base(data)
    {
        this.data = data;
        weapon = base.weapon as DragunovSVD;
    }
}