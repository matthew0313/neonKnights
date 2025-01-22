using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BenelliM4_Item", menuName = "Scriptables/Items/Unstackables/Weapons/Guns/Shotguns/BenelliM4", order = 0)]
public class BenelliM4_ItemData : ShotgunItemData
{
    public override Item Create()
    {
        return new BenelliM4_Item(this);
    }
}
public class BenelliM4_Item : ShotgunItem
{
    new public readonly BenelliM4_ItemData data;
    new public BenelliM4 weapon => base.weapon as BenelliM4;
    public BenelliM4_Item(BenelliM4_ItemData data) : base(data)
    {
        this.data = data;
    }
}