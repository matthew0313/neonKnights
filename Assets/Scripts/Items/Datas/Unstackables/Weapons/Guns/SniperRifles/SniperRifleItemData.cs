using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SniperRifleItemData : GunItemData
{
    
}
public abstract class SniperRifleItem : GunItem
{
    new public readonly SniperRifleItemData data;
    new public SniperRifle weapon => base.weapon as SniperRifle;
    public SniperRifleItem(SniperRifleItemData data) : base(data)
    {
        this.data = data;
    }
    public override void OnWield(Player wielder)
    {
        base.OnWield(wielder);
        wielder.animation.SetAnimationType(AnimationType.SniperRifle);
    }
    public override void OnUnwield(Player wielder)
    {
        base.OnUnwield(wielder);
        wielder.animation.SetAnimationType(AnimationType.None);
    }
}