using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class AccessoryItemData : EquipmentItemData
{

}
public abstract class AccessoryItem : EquipmentItem
{
    new public readonly AccessoryItemData data;
    public AccessoryItem(AccessoryItemData data) : base(data)
    {
        this.data = data;
    }
    public override void OnWieldUpdate()
    {
        base.OnWieldUpdate();
        if (Input.GetMouseButtonDown(0) && UIScanner.ScanUI().Count <= 0)
        {
            if (wielder.inventory.accessorySlot1.item == null)
            {
                wielder.inventory.accessorySlot1.Swap(containedSlot);
            }
            else if(wielder.inventory.accessorySlot2.item == null)
            {
                wielder.inventory.accessorySlot2.Swap(containedSlot);
            }
        }
    }
}