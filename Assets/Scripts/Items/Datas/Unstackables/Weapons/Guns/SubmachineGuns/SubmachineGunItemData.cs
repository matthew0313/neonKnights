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
    public override void OnWield(Player wielder)
    {
        base.OnWield(wielder);
        wielder.animation.SetAnimationType(AnimationType.SubmachineGun);
    }
    public override void OnUnwield(Player wielder)
    {
        base.OnUnwield(wielder);
        wielder.animation.SetAnimationType(AnimationType.None);
    }
}