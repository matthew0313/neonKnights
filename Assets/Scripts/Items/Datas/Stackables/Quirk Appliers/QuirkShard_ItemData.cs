using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Quirk Shard", menuName = "Scriptables/Items/Stackables/Quirk Appliers/Quirk Shard", order = 0)]
public class QuirkShard_ItemData : QuirkApplierItemData
{
    public override Item Create() => new QuirkShard_Item(this);
}
public class QuirkShard_Item : QuirkApplierItem
{
    new public readonly QuirkShard_ItemData data;
    public QuirkShard_Item(QuirkShard_ItemData data) : base(data)
    {
        this.data = data;
    }
    public override ItemModel GetModel()
    {
        QuirkShard_Model model = base.GetModel() as QuirkShard_Model;
        model.SetColor(data.appliedQuirk.quirkColor);
        return model;
    }
}
