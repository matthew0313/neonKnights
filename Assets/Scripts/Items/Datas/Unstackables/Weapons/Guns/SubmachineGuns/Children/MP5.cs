using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MP5_Item", menuName = "Scriptables/Items/Unstackables/Weapons/Guns/SubmachineGuns/MP5", order = 0)]
public class MP5_ItemData : SubmachineGunItemData
{
}

public class MP5_Item : SubmachineGunItem
{
    new public MP5_ItemData data;
    new public MP5 weapon;

    public MP5_Item(MP5_ItemData data) : base(data)
    {
        this.data = data;
        weapon = base.weapon as MP5;
    }
}
