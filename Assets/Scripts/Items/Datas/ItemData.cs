using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class ItemData : ScriptableObject
{
    [Header("Item")]
    [SerializeField] Sprite m_icon;
    [SerializeField] ItemModel m_model;
    public Sprite icon => m_icon;
    public ItemModel model => m_model;
    public abstract Item Create();
    public abstract int maxStack { get; }
    public virtual string unit => "x";
}
public abstract class Item
{
    public readonly ItemData data;
    public Item(ItemData data)
    {
        this.data = data;
    }
    public virtual bool IsStackable(Item other) => other != null && data == other.data;
    public virtual Item Copy() => data.Create();

    public InventorySlot containedSlot = null;
    public Player wielder { get; private set; }
    public virtual void OnInventoryEnter() { }
    public virtual void OnInventoryUpdate() { }
    public virtual void OnInventoryExit() { }
    public virtual void OnWield(Player wielder)
    {
        this.wielder = wielder;
    }
    public virtual void OnWieldUpdate() { }
    public virtual void OnUnwield(Player wielder)
    {
        this.wielder = null;
    }
    public virtual void Save(DataUnit data) { }
    public virtual void Load(DataUnit data) { }
}
[System.Serializable]
public struct ItemDataIntPair
{
    public ItemData data;
    public int count;
}
[System.Serializable]
public struct ItemIntPair
{
    public Item item;
    public int count;
}