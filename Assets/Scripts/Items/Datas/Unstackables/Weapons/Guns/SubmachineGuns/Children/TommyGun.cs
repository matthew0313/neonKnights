using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TommyGun_Item", menuName = "Scriptables/Items/Unstackables/Weapons/Guns/SubmachineGuns/TommyGun", order = 0)]
public class TommyGun_ItemData : SubmachineGunItemData
{
    // Add any specific data for the TommyGun here
}

public class TommyGun_Item : SubmachineGunItem
{
    new public TommyGun_ItemData data;
    new public TommyGun weapon;

    public TommyGun_Item(TommyGun_ItemData data) : base(data)
    {
        this.data = data;
        weapon = base.weapon as TommyGun;
    }
}
