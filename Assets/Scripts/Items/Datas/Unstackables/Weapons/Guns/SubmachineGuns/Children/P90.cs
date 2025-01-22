using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "P90_Item", menuName = "Scriptables/Items/Unstackables/Weapons/Guns/SubmachineGuns/P90", order = 0)]
public class P90_ItemData : SubmachineGunItemData
{
    // Add any specific data for the P90 here
}

public class P90_Item : SubmachineGunItem
{
    new public P90_ItemData data;
    new public P90 weapon;

    public P90_Item(P90_ItemData data) : base(data)
    {
        this.data = data;
        weapon = base.weapon as P90;
    }
}
