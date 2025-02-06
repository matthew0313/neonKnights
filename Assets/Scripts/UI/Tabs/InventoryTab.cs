using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class InventoryTab : Tab
{
    [SerializeField] RectTransform rectTransform;

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
        Debug.Log(player.slotCount);
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
    Tween tween;
    public override void OpenTab()
    {
        base.OpenTab();
        if (tween != null) tween.Kill();
        tween = rectTransform.DOPivot(new Vector2(0.5f, 1.1f), 0.25f).OnUpdate(() =>
        {
            rectTransform.anchoredPosition = new Vector2(0.0f, 0.0f);
        });
    }
    public override void CloseTab()
    {
        if (tween != null) tween.Kill();
        tween = rectTransform.DOPivot(new Vector2(0.5f, 0.0f), 0.25f).OnUpdate(() =>
        {
            rectTransform.anchoredPosition = new Vector2(0.0f, 0.0f);
        }).OnComplete(() =>
        {
            base.CloseTab();
        });
    }
}
