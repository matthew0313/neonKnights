using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Quirk Applier", menuName = "Scriptables/Items/Stackables/Quirk Appliers/Quirk Applier", order = 0)]
public class QuirkApplierItemData : StackableItemData
{
    [Header("QuirkShard")]
    [SerializeField] QuirkData m_appliedQuirk;
    [SerializeField] int m_shardRequired = 3;
    [SerializeField] int m_upgradeShardRequired = 1;
    [SerializeField] Sound m_applySound;
    public QuirkData appliedQuirk => m_appliedQuirk;
    public int shardRequired => m_shardRequired;
    public int upgradeShardRequired => m_upgradeShardRequired;
    public Sound applySound => m_applySound;
    public override Item Create() => new QuirkApplierItem(this);
}
public class QuirkApplierItem : StackableItem, IApplied
{
    new public readonly QuirkApplierItemData data;
    public QuirkApplierItem(QuirkApplierItemData data) : base(data)
    {
        this.data = data;
    }
    public bool IsAppliableTo(Item item)
    {
        if(item is IQuirkable tmp)
        {
            if (data.appliedQuirk.IsAddable(tmp)) return true;
            else
            {
                Quirk found = tmp.quirks.Find((quirk) => quirk.data == data.appliedQuirk);
                if (found != null && found.IsUpgradable())
                {
                    return true;
                }
            }
        }
        return false;
    }
    public void ApplyTo(Item appliedTo)
    {
        if (data.appliedQuirk == null || !(appliedTo is IQuirkable tmp)) return;
        if(data.appliedQuirk.IsAddable(tmp))
        {
            if(containedSlot.count >= data.shardRequired)
            {
                AudioManager.Instance.PlaySound(data.applySound);
                containedSlot.count -= data.shardRequired;
                Quirk tmp2 = data.appliedQuirk.Create();
                tmp2.SetRandom();
                (appliedTo as IQuirkable).AddQuirk(tmp2);
            }
        }
        else
        {
            Quirk found = tmp.quirks.Find((quirk) => quirk.data == data.appliedQuirk);
            if (found != null && found.IsUpgradable())
            {
                if(containedSlot.count >= data.upgradeShardRequired)
                {
                    AudioManager.Instance.PlaySound(data.applySound);
                    containedSlot.count -= data.upgradeShardRequired;
                    found.Upgrade();
                }
            }
        }
    }
}
