using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Quirk Shard", menuName = "Scriptables/Items/Stackables/Quirk Shard", order = 0)]
public class QuirkShard_ItemData : StackableItemData
{
    [Header("QuirkShard")]
    [SerializeField] QuirkData m_shardOf;
    [SerializeField] int m_shardRequired = 3;
    [SerializeField] Sound m_applySound;
    public QuirkData shardOf => m_shardOf;
    public int shardRequired => m_shardRequired;
    public Sound applySound => m_applySound;
    public override Item Create() => new QuirkShard_Item(this);
}
public class QuirkShard_Item : StackableItem, IApplied
{
    new public readonly QuirkShard_ItemData data;
    public QuirkShard_Item(QuirkShard_ItemData data) : base(data)
    {
        this.data = data;
    }
    public QuirkData quirkShardOf => data.shardOf;
    public bool IsAppliableTo(Item item)
    {
        return item is IQuirkable tmp && quirkShardOf.IsAddable(tmp);
    }
    public void ApplyTo(Item appliedTo)
    {
        if (quirkShardOf == null || !(appliedTo is IQuirkable)) return;
        Debug.Log(containedSlot.count);
        if(quirkShardOf.IsAddable(appliedTo as IQuirkable) && containedSlot.count >= data.shardRequired)
        {
            AudioManager.Instance.PlaySound(data.applySound);
            containedSlot.count -= data.shardRequired;
            Quirk tmp = quirkShardOf.Create();
            tmp.SetRandom();
            (appliedTo as IQuirkable).AddQuirk(tmp);
        }
    }
    public override ItemModel GetModel()
    {
        QuirkShard_Model tmp = base.GetModel() as QuirkShard_Model;
        tmp.SetColor(quirkShardOf.quirkColor);
        return tmp;
    }
}
[System.Serializable]
public class QuirkShardSave
{
    public QuirkData quirkShardOf;
}
