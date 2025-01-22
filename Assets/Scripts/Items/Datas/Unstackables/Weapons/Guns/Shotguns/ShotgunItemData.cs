using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShotgunItemData : GunItemData
{
    
}
public abstract class ShotgunItem : GunItem
{
    new public readonly ShotgunItemData data;
    new public Shotgun weapon => base.weapon as Shotgun;
    public ShotgunItem(ShotgunItemData data) : base(data)
    {
        this.data = data;
    }
    public override void OnWield(Player wielder)
    {
        base.OnWield(wielder);
        wielder.animation.SetAnimationType(AnimationType.Shotgun);
    }
    public override void OnUnwield(Player wielder)
    {
        base.OnUnwield(wielder);
        wielder.animation.SetAnimationType(AnimationType.None);
    }
}