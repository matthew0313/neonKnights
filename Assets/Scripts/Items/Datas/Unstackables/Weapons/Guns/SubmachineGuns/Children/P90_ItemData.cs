using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "P90_Item", menuName = "Scriptables/Items/Unstackables/Weapons/Guns/SubmachineGuns/P90", order = 0)]
public class P90_ItemData : SubmachineGunItemData
{
    public override Item Create()
    {
        return new P90_Item(this);
    }
}

public class P90_Item : SubmachineGunItem
{
    new public readonly P90_ItemData data;
    new public P90 weapon => base.weapon as P90;

    public P90_Item(P90_ItemData data) : base(data)
    {
        this.data = data;
    }
}
