using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Quirk_Saw", menuName = "Scriptables/Quirks/Guns/Saw", order = 0)]
public class QuirkData_Saw : QuirkData
{
    public override bool IsAddable(IQuirkable target)
    {
        return base.IsAddable(target) && target is GunItem;
    }
    public override Quirk Create()
    {
        return new Quirk_Saw(this);
    }
}
public class Quirk_Saw : Quirk
{
    new public readonly QuirkData_Saw data;
    public Quirk_Saw(QuirkData_Saw data) : base(data)
    {
        this.data = data;
    }
    public override void OnQuirkAdd(IQuirkable target)
    {
        base.OnQuirkAdd(target);
        (target as GunItem).range = Mathf.Min(3, (target as GunItem).range);
        (target as GunItem).onFireStart += OnFireStart;
        (target as GunItem).onShotHit += OnShotHit;
        (target as GunItem).onFireEnd += OnFireEnd;
    }
    List<GunFire> notRefunded = new();
    void OnFireStart(GunFire fire)
    {
        if(fire.fireInfo.source == GunFireSource.GunFire)
        {
            notRefunded.Add(fire);
        }
    }
    void OnShotHit(GunShot shot, GunHitInfo info)
    {
        if (notRefunded.Contains(shot.origin))
        {
            (target as GunItem).mag = Mathf.Min((target as GunItem).magSize, (target as GunItem).mag += shot.origin.fireInfo.consumedBullet);
            notRefunded.Remove(shot.origin);
        }
    }
    void OnFireEnd(GunFire fire)
    {
        if (notRefunded.Contains(fire)) notRefunded.Remove(fire);
    }
    public override LangText Describe()
    {
        return new LangText()
        {
            en = $"3 max range, no ammo consumed on hit",
            kr = $"최대 범위 3, 탄환이 맞으면 총알 소비 X"
        };
    }
}