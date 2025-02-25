using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player_Inventory : Inventory, ICutsceneCallbackReceiver
{
    Player origin;
    [SerializeField] Transform m_equipAnchor;
    public Transform equipAnchor => m_equipAnchor;

    [SerializeField] ItemDataIntPair[] startContents;
    [SerializeField] float itemDropForce = 4.0f;
    public const int beltLength = 6;

    internal InventorySlot equippedSlot;
    internal int equippedSlotIndex = -1;
    public Action onEquippedSlotChange;

    internal Item equipped;
    public Action<Item> onEquippedItemChange;

    internal InventorySlot chestplateSlot;
    internal ChestplateItem chestplate;
    public Action<ChestplateItem> onChestplateChange;

    internal InventorySlot accessorySlot1;
    internal InventorySlot accessorySlot2;
    internal AccessoryItem accessory1, accessory2;
    public Action<AccessoryItem> onAccessory1Change, onAccessory2Change;

    public bool canSwapSlot = true;
    public void OnAwake()
    {
        chestplateSlot = new InventorySlot(this) { slotRestriction = (Item item) => item == null || item is ChestplateItem };
        accessorySlot1 = new InventorySlot(this) { slotRestriction = (Item item) => item == null || item is AccessoryItem };
        accessorySlot2 = new InventorySlot(this) { slotRestriction = (Item item) => item == null || item is AccessoryItem };
        Init();
        origin = GetComponent<Player>();
        ChangeEquipSlot(0);

        chestplateSlot.onItemChange += OnChestplateSlotUpdate;
        accessorySlot1.onItemChange += OnAccessorySlot1Update;
        accessorySlot2.onItemChange += OnAccessorySlot2Update;
    }
    public void OnStart()
    {
        for (int i = 0; i < Mathf.Min(startContents.Length, slotCount); i++)
        {
            if (startContents[i].data == null || startContents[i].count <= 0) continue; 
            slots[i].SetItem(startContents[i].data.Create());
            slots[i].count = startContents[i].count;
        }
    }
    public void OnUpdate()
    {
        if (inCutscene) return;

        SlotChangeCheckUpdate();
        if (equipped != null) equipped.OnWieldUpdate();
        if (chestplate != null) chestplate.OnEquipUpdate();
        if (accessory1 != null) accessory1.OnEquipUpdate();
        if (accessory2 != null) accessory2.OnEquipUpdate();
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
        if (!canSwapSlot) return;
        if (slotIndex < 0 || slotIndex >= beltLength || slotIndex == equippedSlotIndex) return;
        if(equippedSlot != null)
        {
            equippedSlot.onItemChange -= OnEquippedSlotUpdate;
        }
        equippedSlot = slots[slotIndex];
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
    public void AddItem_DropRest(Item item, int count)
    {
        count = Insert(item, count);
        if (count > 0) DropItem(item.Copy(), count);
    }
    public void DropItem(Item item, int count)
    {
        DroppedItem tmp = DroppedItem.Create(item, count);
        tmp.transform.position = transform.position;
        tmp.rb.AddForce(origin.movement.rotator.forward * itemDropForce, ForceMode.VelocityChange);
    }
    void OnChestplateSlotUpdate()
    {
        ChangeChestplate(chestplateSlot.item as ChestplateItem);
    }
    void ChangeChestplate(ChestplateItem item)
    {
        if (chestplate != null) chestplate.OnUnequip(origin);
        chestplate = item;
        onChestplateChange?.Invoke(chestplate);
        if (chestplate != null) chestplate.OnEquip(origin);
    }
    void OnAccessorySlot1Update()
    {
        ChangeAccessory1(accessorySlot1.item as AccessoryItem);
    }
    void OnAccessorySlot2Update()
    {
        ChangeAccessory2(accessorySlot2.item as AccessoryItem);
    }
    void ChangeAccessory1(AccessoryItem item)
    {
        if (accessory1 != null) accessory1.OnUnequip(origin);
        accessory1 = item;
        onAccessory1Change?.Invoke(accessory1);
        if (accessory1 != null) accessory1.OnEquip(origin);
    }
    void ChangeAccessory2(AccessoryItem item)
    {
        if (accessory2 != null) accessory2.OnUnequip(origin);
        accessory2 = item;
        onAccessory2Change?.Invoke(accessory2);
        if (accessory2 != null) accessory2.OnEquip(origin);
    }

    bool inCutscene = false;
    public void OnCutsceneStart()
    {
        inCutscene = true;
    }

    public void OnCutsceneEnd()
    {
        inCutscene = false;
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (chestplate != null) chestplate.Delete();
        if (accessory1 != null) accessory1.Delete();
        if (accessory2 != null) accessory2.Delete();
    }
}