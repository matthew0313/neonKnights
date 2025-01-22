using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PistolItemData : GunItemData
{
    
}
public abstract class PistolItem : GunItem
{
    new public readonly PistolItemData data;
    new public Pistol weapon => base.weapon as Pistol;
    public PistolItem(PistolItemData data) : base(data)
    {
        this.data = data;
    }
    public override void OnWield(Player wielder)
    {
        base.OnWield(wielder);
        wielder.animation.SetAnimationType(AnimationType.Pistol);
    }
    public override void OnUnwield(Player wielder)
    {
        base.OnUnwield(wielder);
        wielder.animation.SetAnimationType(AnimationType.None);
    }
}