using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class ItemData : ScriptableObject
{
    [Header("Item")]
    [SerializeField] LangText m_itemName;
    [SerializeField] LangText m_description;
    [SerializeField] ItemModel m_model;
    [SerializeField] ItemCategory m_category;
    [SerializeField] bool m_canDrop = true;
    public LangText itemName => m_itemName;
    public LangText description => m_description;
    public ItemModel model => m_model;
    public ItemCategory category => m_category;
    public bool canDrop => m_canDrop;
    public abstract Item Create();
    public abstract int maxStack { get; }
    public virtual string unit => "x";
}
public abstract class Item
{
    public readonly ItemData data;
    RenderTexture m_render = null;
    ItemRenderSpace renderSpace = null;
    public RenderTexture render
    {
        get
        {
            if(m_render == null)
            {
                m_render = new RenderTexture(256, 256, 16, UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_UNorm);
                m_render.Create();
                renderSpace = ItemRenderSpace.GetSpace();
                renderSpace.SetTarget(m_render);
                renderSpace.RenderItem(GetModel());
                Debug.Log(renderSpace.name + " Create");
            }
            return m_render;
        }
    }
    public virtual AnimationType heldAnimation => AnimationType.None;
    public virtual RotateMode rotateMode => RotateMode.Free;
    public virtual LangText GetName() => data.itemName;
    public virtual LangText GetDescription() => data.description;
    public Item(ItemData data)
    {
        this.data = data;
        ItemManager.Instance.AddItem(this);
    }
    public virtual bool IsStackable(Item other) => other != null && data == other.data;
    public virtual Item Copy() => data.Create();
    public Player wielder { get; private set; }


    public Action<Player> onWield, onUnwield;
    public Action onWieldUpdate;
    public Action onUpdate;
    public Action onModelUpdate;
    public Action<ItemModel> modifyModel;
    public virtual void Update()
    {
        onUpdate?.Invoke();
    }
    public InventorySlot containedSlot { get; private set; } = null;
    public virtual void OnSlotEnter(InventorySlot slot)
    {
        containedSlot = slot;
    }
    public virtual void OnSlotExit(InventorySlot slot)
    {
        containedSlot = null;
    }
    public virtual void OnWield(Player wielder)
    {
        this.wielder = wielder;
        onWield?.Invoke(wielder);
    }
    public virtual void OnWieldUpdate()
    {
        onWieldUpdate?.Invoke();
    }
    public virtual void OnUnwield(Player wielder)
    {
        this.wielder = null;
        onUnwield?.Invoke(wielder);
    }
    public virtual void Delete()
    {
        ItemManager.Instance.RemoveItem(this);
        if (renderSpace != null)
        {
            renderSpace.Release();
            Debug.Log(renderSpace.name + " Delete");
        }
        if (render != null) render.Release();
    }
    public virtual void Save(DataUnit data) { }
    public virtual void Load(DataUnit data) { }
    static ItemModel dummyModel;
    public virtual ItemModel GetModel()
    {
        if (data.model == null)
        {
            if (dummyModel == null) dummyModel = Resources.Load<ItemModel>("DummyModel");
            return dummyModel.Instantiate();
        }
        ItemModel tmp = data.model.Instantiate();
        modifyModel?.Invoke(tmp);
        return tmp;
    }
    public void UpdateModel()
    {
        if (renderSpace != null) renderSpace.RenderItem(GetModel());
        onModelUpdate?.Invoke();
    }
    public static LangText CategoryToLangText(ItemCategory category)
    {
        switch (category)
        {
            case ItemCategory.Weapon: return new LangText()
            {
                en = "Weapon",
                kr = "무기"
            };
            case ItemCategory.Gun: return new LangText()
            {
                en = "Gun",
                kr = "총"
            };
            case ItemCategory.Pistol: return new LangText()
            {
                en = "Pistol",
                kr = "권총"
            };
            case ItemCategory.SubmachineGun: return new LangText()
            {
                en = "Submachine Gun",
                kr = "기관단총"
            };
            case ItemCategory.SniperRifle: return new LangText()
            {
                en = "Sniper Rifle",
                kr = "저격총"
            };
            case ItemCategory.Shotgun: return new LangText()
            {
                en = "Shotgun",
                kr = "샷건"
            };
            case ItemCategory.Consumable: return new LangText()
            {
                en = "Consumable",
                kr = "소모품"
            };
            case ItemCategory.Potion: return new LangText()
            {
                en = "Potion",
                kr = "포션"
            };
            case ItemCategory.Throwable: return new LangText()
            {
                en = "Throwable",
                kr = "투척 아이템"
            };
            case ItemCategory.Equipment: return new LangText()
            {
                en = "Equipment",
                kr = "장비품"
            };
            case ItemCategory.Chestplate: return new LangText()
            {
                en = "Chestplate",
                kr = "흉갑"
            };
            case ItemCategory.Accessory: return new LangText()
            {
                en = "Accessory",
                kr = "장신구"
            };
            case ItemCategory.Melee: return new LangText()
            {
                en = "Melee",
                kr = "근접 무기"
            };
            case ItemCategory.KeyItem: return new LangText()
            {
                en = "Key Item",
                kr = "중요함"
            };
            default: return new LangText()
            {
                en = "???",
                kr = "???"
            };
        }
    }
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
    Pistol = 1<<2,
    SubmachineGun = 1<<3,
    SniperRifle = 1<<4,
    Shotgun = 1<<5,
    Consumable = 1<<6,
    Potion = 1<<7,
    Throwable = 1<<8,
    Equipment = 1<<9,
    Chestplate = 1<<10,
    Accessory = 1<<11,
    Melee = 1<<12,
    KeyItem = 1<<13,
}