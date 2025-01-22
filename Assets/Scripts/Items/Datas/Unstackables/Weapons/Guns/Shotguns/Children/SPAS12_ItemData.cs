using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SPAS12_Item", menuName = "Scriptables/Items/Unstackables/Weapons/Guns/Shotguns/SPAS12", order = 0)]
public class SPAS12_ItemData : ShotgunItemData
{
    public override Item Create()
    {
        return new SPAS12_Item(this);
    }
}
public class SPAS12_Item : ShotgunItem
{
    new public readonly SPAS12_ItemData data;
    new public SPAS12 weapon => base.weapon as SPAS12;
    public SPAS12_Item(SPAS12_ItemData data) : base(data)
    {
        this.data = data;
    }
}