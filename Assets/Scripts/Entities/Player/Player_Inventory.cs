using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Inventory))]
public class Player_Inventory : MonoBehaviour
{
    Player origin;
    [SerializeField] InventorySlotUIGrabber grabber;
    [SerializeField] Transform m_equipAnchor;
    public Transform equipAnchor => m_equipAnchor;

    [SerializeField] ItemDataIntPair[] startContents;
    public const int beltLength = 6;

    internal Inventory inventory;
    internal InventorySlot equippedSlot;
    internal int equippedSlotIndex = -1;
    internal Item equipped;

    public Action<Item> onEquippedItemChange;
    public Action onEquippedSlotChange;
    public void OnAwake()
    {
        origin = GetComponent<Player>();
        inventory = GetComponent<Inventory>();
        if (grabber != null) grabber.onItemDrop += DropItem;
        ChangeEquipSlot(0);
    }
    public void OnStart()
    {
        for (int i = 0; i < Mathf.Min(startContents.Length, inventory.slotCount); i++)
        {
            inventory.slots[i].SetItem(startContents[i].data.Create());
            inventory.slots[i].count = startContents[i].count;
        }
    }
    public void OnUpdate()
    {
        SlotChangeCheckUpdate();
    }

    const float rapidSlotChangePressTime = 0.5f;
    const float rapidSlotChangeRate = 0.1f;
    float pressCounter = 0.0f, changeCounter = 0.0f;
    SlotChangeInput lastInput = SlotChangeInput.Stay;
    void SlotChangeCheckUpdate()
    {
        if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            for (int i = 0; i < beltLength; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    ChangeEquipSlot(i);
                    break;
                }
            }
        }
        SlotChangeInput input = InputManager.GetSlotChangeInput();
        if (input == SlotChangeInput.MoveLeft && equippedSlotIndex > 0)
        {
            if (lastInput != SlotChangeInput.MoveLeft)
            {
                ChangeEquipSlot(equippedSlotIndex - 1);
                pressCounter = 0.0f;
            }
            else
            {
                if (pressCounter < rapidSlotChangePressTime) pressCounter += Time.deltaTime;
                else
                {
                    if (changeCounter < rapidSlotChangeRate) changeCounter += Time.deltaTime;
                    else
                    {
                        ChangeEquipSlot(equippedSlotIndex - 1);
                        changeCounter = 0.0f;
                    }
                }
            }
        }
        else if (input == SlotChangeInput.MoveRight && equippedSlotIndex < beltLength - 1)
        {
            if (lastInput != SlotChangeInput.MoveRight)
            {
                ChangeEquipSlot(equippedSlotIndex + 1);
                pressCounter = 0.0f;
            }
            else
            {
                if (pressCounter < rapidSlotChangePressTime) pressCounter += Time.deltaTime;
                else
                {
                    if (changeCounter < rapidSlotChangeRate) changeCounter += Time.deltaTime;
                    else
                    {
                        ChangeEquipSlot(equippedSlotIndex + 1);
                        changeCounter = 0.0f;
                    }
                }
            }
        }
        else pressCounter = 0.0f;
        lastInput = input;
    }
    public void ChangeEquipSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= beltLength || slotIndex == equippedSlotIndex) return;
        if(equippedSlot != null)
        {
            equippedSlot.onItemChange -= OnEquippedSlotUpdate;
        }
        equippedSlot = inventory.slots[slotIndex];
        equippedSlotIndex = slotIndex;
        equippedSlot.onItemChange += OnEquippedSlotUpdate;
        onEquippedSlotChange?.Invoke();
        OnEquippedSlotUpdate();
    }
    void OnEquippedSlotUpdate()
    {
        ChangeEquippedItem(equippedSlot.item);
    }
    void ChangeEquippedItem(Item item)
    {
        if (equipped != null) equipped.OnUnwield(origin);
        equipped = item;
        onEquippedItemChange?.Invoke(equipped);
        if (equipped != null) equipped.OnWield(origin);
    }
    void DropItem(Item item, int count)
    {

    }
}