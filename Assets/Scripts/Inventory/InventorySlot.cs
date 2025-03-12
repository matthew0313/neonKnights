using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventorySlot
{
    public Action onItemChange, onCountChange;
    public Func<Item, bool> slotRestriction;
    public Item item { get; private set; }
    int m_count = 0;
    public InventorySlot() { }

    public readonly Inventory dedicatedInventory;
    public InventorySlot(Inventory dedicatedInventory)
    {
        this.dedicatedInventory = dedicatedInventory;
    }
    public int Insert(Item item, int count)
    {
        if (this.item == null)
        {
            if (SetItem(item))
            {
                this.count = Mathf.Min(item.data.maxStack, this.count + count);
                count -= this.count;
            }
        }
        else if (this.item.IsStackable(item) && item.IsStackable(this.item))
        {
            int prev = this.count;
            this.count = Mathf.Min(this.item.data.maxStack, this.count + count);
            count -= this.count - prev;
        }
        return count;
    }
    public int TryInsert(Item item, int count)
    {
        if (this.item == null)
        {
            if (CanSetItem(item))
            {
                count -= Mathf.Min(item.data.maxStack, this.count + count);
            }
        }
        else if (this.item.IsStackable(item) && item.IsStackable(this.item))
        {
            count -= Mathf.Min(this.item.data.maxStack, this.count + count) - this.count;
        }
        return count;
    }
    public bool CanSetItem(Item item)
    {
        if (slotRestriction != null && slotRestriction.Invoke(item) == false) return false;
        return true;
    }
    public bool SetItem(Item item)
    {
        if (!CanSetItem(item)) return false;
        ForcedSetItem(item);
        return true;
    }
    public bool Swap(InventorySlot slot)
    {
        if (!slot.CanSetItem(item) || !CanSetItem(slot.item)) return false;
        Item tmp = item;
        int tmp2 = count;
        ForcedSetItem(slot.item);
        count = slot.count;
        slot.ForcedSetItem(tmp);
        slot.count = tmp2;
        return true;
    }
    public void ForcedSetItem(Item item)
    {
        if(this.item != null)
        {
            this.item.OnSlotExit(this);
        }
        this.item = item;
        if (this.item != null)
        {
            this.item.OnSlotEnter(this);
        }

        onItemChange?.Invoke();
    }
    public int count
    {
        get
        {
            return m_count;
        }
        set
        {
            m_count = value;
            onCountChange?.Invoke();
            if(m_count == 0)
            {
                if (item != null) item.Delete();
                ForcedSetItem(null);
            }
        }
    }
    public InventorySlotSaveData Save()
    {
        InventorySlotSaveData data = new();
        if (item != null)
        {
            data.item = item.data;
            item.Save(data.data);
        }
        data.count = count;
        return data;
    }
    public void Load(InventorySlotSaveData data)
    {
        if (data == null || data.item == null) return;
        Item item = data.item.Create();
        ForcedSetItem(item);
        item.Load(data.data);
        count = data.count;
    }
}
[System.Serializable]
public class InventorySlotSaveData
{
    public ItemData item;
    public DataUnit data = new();
    public int count;
}
