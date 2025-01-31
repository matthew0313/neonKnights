using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Android.Gradle.Manifest;
using UnityEngine;

[CreateAssetMenu(fileName = "Grenade Mk1", menuName = "Scriptables/Items/Stackables/Throwables/Projectile Throwers/Grenades/Grenade Mk1", order = 0)]
public class Grenade_Mk1_ItemData : GrenadeItemData
{
    public override Item Create()
    {
        return new Grenade_Mk1_Item(this);
    }
}
public class Grenade_Mk1_Item : GrenadeItem
{
    new public readonly Grenade_Mk1_ItemData data;
    public Grenade_Mk1_Item(Grenade_Mk1_ItemData data) : base(data)
    {
        this.data = data;
    }
}