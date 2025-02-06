using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class ChestplateItemData : EquipmentItemData
{

}
public abstract class ChestplateItem : EquipmentItem
{
    new public readonly ChestplateItemData data;
    public ChestplateItem(ChestplateItemData data) : base(data)
    {
        this.data = data;
    }
    public override void OnWieldUpdate()
    {
        base.OnWieldUpdate();
        if (Input.GetMouseButtonDown(0) && UIScanner.ScanUI().Count <= 0)
        {
            if(wielder.inventory.chestplateSlot.item == null)
            {
                wielder.inventory.chestplateSlot.Swap(containedSlot);
            }
        }
    }
}