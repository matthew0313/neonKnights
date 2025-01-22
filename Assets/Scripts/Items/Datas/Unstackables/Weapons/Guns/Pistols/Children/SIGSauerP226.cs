using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "SIGSauerP226", menuName = "Scriptables/Items/Unstackables/Weapons/Guns/Pistols/SIGSauerP226", order = 0)]
public class SIGSauerP226_ItemData : PistolItemData
{
    public override Item Create()
    {
        return new SIGSauerP226_Item(this);
    }
}

public class SIGSauerP226_Item : PistolItem
{
    new public readonly SIGSauerP226_ItemData data;
    new public readonly SIGSauerP226 weapon;

    public SIGSauerP226_Item(SIGSauerP226_ItemData data) : base(data)
    {
        this.data = data;
        weapon = base.weapon as SIGSauerP226;
    }
}
