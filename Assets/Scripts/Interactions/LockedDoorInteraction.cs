using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedDoorInteraction : DoorInteraction, ISavable
{
    [Header("Locked Door")]
    [SerializeField] ItemData requiredItem;
    [SerializeField] bool consumeOnUnlock = true;
    [SerializeField] Sound unlockSound;
    [SerializeField] SaveID id;

    bool unlocked = false;
    public override LangText interactionName => unlocked ? base.interactionName : player.inventory.equipped != null && player.inventory.equipped.data == requiredItem ? new LangText()
    {
        en = "Unlock Door",
        kr = "잠금 해제하기"
    } : new LangText()
    {
        en = $"Requires \'{requiredItem.itemName.text}\'.",
        kr = $"\'{requiredItem.itemName.text}\'가 필요하다."
    };
    public override void OnInteract()
    {
        if(unlocked) base.OnInteract();
        else
        {
            if(player.inventory.equipped.data == requiredItem)
            {
                unlocked = true;
                if (consumeOnUnlock) player.inventory.equippedSlot.count--;
                AudioManager.Instance.PlaySound(unlockSound);
            }
        }
    }
    public void Save(SaveData data)
    {
        data.mapElements[id.value] = new();
        data.mapElements[id.value].bools["unlocked"] = unlocked;
    }
    public void Load(SaveData data)
    {
        if(data.mapElements.TryGetValue(id.value, out DataUnit tmp))
        {
            if (tmp.bools.TryGetValue("unlocked", out bool val))
            {
                unlocked = val;
            }
        }
    }
}