using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TommyGun_Item", menuName = "Scriptables/Items/Unstackables/Weapons/Guns/SubmachineGuns/TommyGun", order = 0)]
public class TommyGun_ItemData : SubmachineGunItemData
{
    public override Item Create()
    {
        return new TommyGun_Item(this);
    }
}

public class TommyGun_Item : SubmachineGunItem
{
    new public readonly TommyGun_ItemData data;
    new public TommyGun weapon => base.weapon as TommyGun;

    public TommyGun_Item(TommyGun_ItemData data) : base(data)
    {
        this.data = data;
    }
}
