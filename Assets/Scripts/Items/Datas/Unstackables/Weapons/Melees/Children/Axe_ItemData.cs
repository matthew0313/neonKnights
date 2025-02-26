using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Axe", menuName = "Scriptables/Items/Unstackables/Weapons/Melees/Axe", order = 0)]
public class Axe_ItemData : MeleeItemData
{
    public override Item Create()
    {
        return new Axe_Item(this);
    }
}
public class Axe_Item : MeleeItem
{
    new public readonly Axe_ItemData data;
    public Axe_Item(Axe_ItemData data) : base(data)
    {
        this.data = data;
    }
    public override AnimationType heldAnimation => AnimationType.Axe;
}