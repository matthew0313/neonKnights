using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Ammo Pack Item Data", menuName = "Scriptables/Items/Stackables/Ammo Pack", order = 0)]
public class AmmoPackItemData : StackableItemData
{
    [Header("Ammo Pack")]
    [SerializeField] float m_ammoRefillPercentage;
    [SerializeField] Sound m_refillSound;
    public float ammoRefillPercentage => m_ammoRefillPercentage;
    public Sound refillSound => m_refillSound;
    public override Item Create() => new AmmoPackItem(this);
}
public class AmmoPackItem : StackableItem, IApplied, IStatDisplayed
{
    new public readonly AmmoPackItemData data;
    public AmmoPackItem(AmmoPackItemData data) : base(data)
    {
        this.data = data;
    }

    public void ApplyTo(Item item)
    {
        (item as GunItem).RefillAmmo(new GunAmmoRefillInfo() { refillPercentage = data.ammoRefillPercentage });
        containedSlot.count--;
        AudioManager.Instance.PlaySound(data.refillSound);
    }
    public bool IsAppliableTo(Item item)
    {
        return item is GunItem;
    }
    public IEnumerable<LangText> GetStats()
    {
        yield return new()
        {
            en = $"Ammo Refill: {data.ammoRefillPercentage}%",
            kr = $"탄환 보충량: {data.ammoRefillPercentage}%"
        };
    }
}
