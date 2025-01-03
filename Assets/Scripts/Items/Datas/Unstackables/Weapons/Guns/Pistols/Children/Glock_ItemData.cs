using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "Glock", menuName = "Scriptables/Items/Unstackables/Weapons/Guns/Pistols/Glock", order = 0)]
public class Glock_ItemData : PistolItemData
{
    public override Item Create()
    {
        return new Glock_Item(this);
    }
}
public class Glock_Item : PistolItem
{
    new public readonly Glock_ItemData data;
    new public readonly Glock weapon;
    public Glock_Item(Glock_ItemData data) : base(data)
    {
        this.data = data;
        weapon = base.weapon as Glock;
    }
}