using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "DesertEagle", menuName = "Scriptables/Items/Unstackables/Weapons/Guns/Pistols/DesertEagle", order = 0)]
public class DesertEagle_ItemData : PistolItemData
{
    public override Item Create()
    {
        return new DesertEagle_Item(this);
    }
}

public class DesertEagle_Item : PistolItem
{
    new public readonly DesertEagle_ItemData data;
    new public readonly DesertEagle weapon;

    public DesertEagle_Item(DesertEagle_ItemData data) : base(data)
    {
        this.data = data;
        weapon = base.weapon as DesertEagle;
    }
}
