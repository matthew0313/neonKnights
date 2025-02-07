using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DroppedItem))]
public class DroppedItemInteraction : Interaction
{
    public override LangText interactionName => new()
    {
        en = $"Pick up{(origin.slot.count > 1 ? $" ({origin.slot.count})" : null)}",
        kr = $"줍기{(origin.slot.count > 1 ? $" ({origin.slot.count})" : null)}"
    };

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