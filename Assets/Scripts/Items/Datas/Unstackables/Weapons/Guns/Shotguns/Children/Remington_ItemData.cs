using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Remington_Item", menuName = "Scriptables/Items/Unstackables/Weapons/Guns/Shotguns/Remington", order = 0)]
public class Remington_ItemData : ShotgunItemData
{
    public override Item Create()
    {
        return new Remington_Item(this);
    }
}
public class Remington_Item : ShotgunItem
{
    new public readonly Remington_ItemData data;
    new public Remington weapon => base.weapon as Remington;
    public Remington_Item(Remington_ItemData data) : base(data)
    {
        this.data = data;
    }
}