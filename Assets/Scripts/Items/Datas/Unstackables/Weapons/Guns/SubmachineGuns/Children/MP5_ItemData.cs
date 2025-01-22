using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MP5_Item", menuName = "Scriptables/Items/Unstackables/Weapons/Guns/SubmachineGuns/MP5", order = 0)]
public class MP5_ItemData : SubmachineGunItemData
{
    public override Item Create()
    {
        return new MP5_Item(this);
    }
}

public class MP5_Item : SubmachineGunItem
{
    new public readonly MP5_ItemData data;
    new public MP5 weapon => base.weapon as MP5;

    public MP5_Item(MP5_ItemData data) : base(data)
    {
        this.data = data;
    }
}
