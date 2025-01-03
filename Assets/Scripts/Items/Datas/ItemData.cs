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
    [SerializeField] ItemCategory m_category;
    [SerializeField] LangText m_description;
    public Sprite icon => m_icon;
    public ItemModel model => m_model;
    public ItemCategory category => m_category;
    public LangText description => m_description;
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
    public virtual void OnInventoryUpdate() { }
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
[System.Serializable]
[Flags]
public enum ItemCategory
{
    Weapon = 1<<0,
    Gun = 1<<1,
    Pistol = 1<<2
}