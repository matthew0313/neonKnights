using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "Beretta92FS", menuName = "Scriptables/Items/Unstackables/Weapons/Guns/Pistols/Beretta92FS", order = 0)]
public class Beretta92FS_ItemData : PistolItemData
{
    public override Item Create()
    {
        return new Beretta92FS_Item(this);
    }
}

public class Beretta92FS_Item : PistolItem
{
    new public readonly Beretta92FS_ItemData data;
    new public Beretta92FS weapon => base.weapon as Beretta92FS;

    public Beretta92FS_Item(Beretta92FS_ItemData data) : base(data)
    {
        this.data = data;
    }
}
