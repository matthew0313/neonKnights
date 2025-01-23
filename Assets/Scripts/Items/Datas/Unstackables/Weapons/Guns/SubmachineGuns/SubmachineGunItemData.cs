using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SubmachineGunItemData : GunItemData
{
    
}
public abstract class SubmachineGunItem : GunItem
{
    new public readonly SubmachineGunItemData data;
    new public SubmachineGun weapon => base.weapon as SubmachineGun;
    public SubmachineGunItem(SubmachineGunItemData data) : base(data)
    {
        this.data = data;
    }
    public override AnimationType heldAnimation => AnimationType.SubmachineGun;
}