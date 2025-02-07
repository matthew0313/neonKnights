using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DroppedItem))]
public class DroppedItemInteraction : Interaction
{
    DroppedItem m_origin;
    DroppedItem origin
    {
        get
        {
            if(m_origin == null) m_origin = GetComponent<DroppedItem>(); return m_origin;
        }
    }
    public override void OnInteract()
    {
        origin.slot.count = player.inventory.Insert(origin.slot.item, origin.slot.count);
    }
    private void OnDisable()
    {
        RemoveInteraction();
    }
}