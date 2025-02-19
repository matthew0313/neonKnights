using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Quirk Applier", menuName = "Scriptables/Items/Stackables/Quirk Appliers/Quirk Applier", order = 0)]
public class QuirkApplierItemData : StackableItemData
{
    [Header("QuirkShard")]
    [SerializeField] QuirkData m_appliedQuirk;
    [SerializeField] int m_shardRequired = 3;
    [SerializeField] Sound m_applySound;
    public QuirkData appliedQuirk => m_appliedQuirk;
    public int shardRequired => m_shardRequired;
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
        return item is IQuirkable tmp && data.appliedQuirk.IsAddable(tmp);
    }
    public void ApplyTo(Item appliedTo)
    {
        if (data.appliedQuirk == null || !(appliedTo is IQuirkable)) return;
        Debug.Log(containedSlot.count);
        if(data.appliedQuirk.IsAddable(appliedTo as IQuirkable) && containedSlot.count >= data.shardRequired)
        {
            AudioManager.Instance.PlaySound(data.applySound);
            containedSlot.count -= data.shardRequired;
            Quirk tmp = data.appliedQuirk.Create();
            tmp.SetRandom();
            (appliedTo as IQuirkable).AddQuirk(tmp);
        }
    }
}
