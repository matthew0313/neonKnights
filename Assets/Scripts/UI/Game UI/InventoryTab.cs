using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class InventoryTab : AnchorPosTweenTab
{
    [Header("Slots")]
    [SerializeField] Transform slotAnchor;
    [SerializeField] InventorySlotUI slotPrefab;
    [SerializeField] InventorySlotUI chestplateSlot, accessory1Slot, accessory2Slot;

    List<InventorySlotUI> slots = new();
    Player_Inventory player;
    public void OnAwake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Inventory>();
        player.onSlotCountChange += UpdateSlotCount;
        UpdateSlotCount();
        chestplateSlot.Set(player.chestplateSlot);
        accessory1Slot.Set(player.accessorySlot1);
        accessory2Slot.Set(player.accessorySlot2);
    }
    void UpdateSlotCount()
    {
        int i;
        for(i = 0 ; i < player.slotCount - Player_Inventory.beltLength; i++)
        {
            if (slots.Count <= i)
            {
                InventorySlotUI tmp = Instantiate(slotPrefab, slotAnchor);
                tmp.Set(player.slots[i + Player_Inventory.beltLength]);
                slots.Add(tmp);
            }
            slots[i].gameObject.SetActive(true);
        }
        for(; i < slots.Count; i++)
        {
            slots[i].gameObject.SetActive(false);
        }
    }
}
