using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Sword", menuName = "Scriptables/Items/Unstackables/Weapons/Melees/Sword", order = 0)]
public class Sword_ItemData : MeleeItemData
{
    public override Item Create()
    {
        return new Sword_Item(this);
    }
}
public class Sword_Item : MeleeItem
{
    new public readonly Sword_ItemData data;
    public Sword_Item(Sword_ItemData data) : base(data)
    {
        this.data = data;
    }
    public override AnimationType heldAnimation => AnimationType.Sword;
}