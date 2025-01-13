using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "Revolver", menuName = "Scriptables/Items/Unstackables/Weapons/Guns/Pistols/Revolver", order = 0)]
public class Revolver_ItemData : PistolItemData
{
    public override Item Create()
    {
        return new Revolver_Item(this);
    }
}
public class Revolver_Item : PistolItem
{
    new public readonly Revolver_ItemData data;
    new public readonly Revolver weapon;
    public Revolver_Item(Revolver_ItemData data) : base(data)
    {
        this.data = data;
        weapon = base.weapon as Revolver;
    }
}