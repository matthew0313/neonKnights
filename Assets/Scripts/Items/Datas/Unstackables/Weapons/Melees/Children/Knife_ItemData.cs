using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Knife", menuName = "Scriptables/Items/Unstackables/Weapons/Melees/Knife", order = 0)]
public class Knife_ItemData : MeleeItemData
{
    public override Item Create()
    {
        return new Knife_Item(this);
    }
}
public class Knife_Item : MeleeItem
{
    new public readonly Knife_ItemData data;
    public Knife_Item(Knife_ItemData data) : base(data)
    {
        this.data = data;
    }
    public override AnimationType heldAnimation => AnimationType.Knife;
}