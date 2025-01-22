using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "ColtM1911", menuName = "Scriptables/Items/Unstackables/Weapons/Guns/Pistols/ColtM1911", order = 0)]
public class ColtM1911_ItemData : PistolItemData
{
    public override Item Create()
    {
        return new ColtM1911_Item(this);
    }
}

public class ColtM1911_Item : PistolItem
{
    new public readonly ColtM1911_ItemData data;
    new public ColtM1911 weapon => base.weapon as ColtM1911;

    public ColtM1911_Item(ColtM1911_ItemData data) : base(data)
    {
        this.data = data;
    }
}
