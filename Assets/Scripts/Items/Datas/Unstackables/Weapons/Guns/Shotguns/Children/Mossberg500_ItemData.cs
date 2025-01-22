using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Mossberg500_Item", menuName = "Scriptables/Items/Unstackables/Weapons/Guns/Shotguns/Mossberg500", order = 0)]
public class Mossberg500_ItemData : ShotgunItemData
{
    public override Item Create()
    {
        return new Mossberg500_Item(this);
    }
}
public class Mossberg500_Item : ShotgunItem
{
    new public readonly Mossberg500_ItemData data;
    new public Mossberg500 weapon => base.weapon as Mossberg500;
    public Mossberg500_Item(Mossberg500_ItemData data) : base(data)
    {
        this.data = data;
    }
}
