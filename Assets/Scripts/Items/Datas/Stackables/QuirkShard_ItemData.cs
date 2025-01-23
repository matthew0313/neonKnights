using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Quirk Shard", menuName = "Scriptables/Items/Stackables/Quirk Shard", order = 0)]
public class QuirkShard_ItemData : StackableItemData
{
    public override Item Create() => new QuirkShard_Item(this);
}
public class QuirkShard_Item : StackableItem, IApplied
{
    new public readonly QuirkShard_ItemData data;
    public QuirkShard_Item(QuirkShard_ItemData data) : base(data)
    {
        this.data = data;
    }
    public QuirkData quirkShardOf { get; private set; }
    public void SetQuirk(QuirkData quirk)
    {
        quirkShardOf = quirk;
    }
    public override LangText GetName()
    {
        LangText tmp = base.GetName();
        tmp.kr = (quirkShardOf != null ? "[" + quirkShardOf.quirkName.kr + "] " : "공백 ") + tmp.kr;
        tmp.en = (quirkShardOf != null ? "[" + quirkShardOf.quirkName.kr + "] " : "Empty ") + tmp.en;
        return tmp;
    }
    public override LangText GetDescription()
    {
        LangText tmp = base.GetDescription();
        tmp.kr += (quirkShardOf != null ? "\n" + "[" + quirkShardOf.quirkName.kr + "]: " + quirkShardOf.quirkDescription.kr : "붙이는 쿼크가 없습니다.");
        tmp.en += (quirkShardOf != null ? "\n" + "[" + quirkShardOf.quirkName.en + "]: " + quirkShardOf.quirkDescription.en : "This shard does not give any quirk.");
        return tmp;
    }
    public override bool IsStackable(Item other)
    {
        return base.IsStackable(other) && (other as QuirkShard_Item).quirkShardOf == quirkShardOf;
    }
    public void ApplyTo(Item appliedTo)
    {
        if (quirkShardOf == null || !(appliedTo is IQuirkable)) return;
        Debug.Log(containedSlot.count);
        if(quirkShardOf.IsAddable(appliedTo as IQuirkable) && containedSlot.count >= 3)
        {
            containedSlot.count -= 3;
            Quirk tmp = quirkShardOf.Create();
            tmp.SetRandom();
            (appliedTo as IQuirkable).AddQuirk(tmp);
        }
    }
    public override void Save(DataUnit data)
    {
        base.Save(data);
        data.strings["quirkShardOf"] = JsonUtility.ToJson(new QuirkShardSave() { quirkShardOf = quirkShardOf });
    }
    public override void Load(DataUnit data)
    {
        base.Load(data);
        if (data.strings.TryGetValue("quirkShardOf", out string tmp)) quirkShardOf = JsonUtility.FromJson<QuirkShardSave>(tmp).quirkShardOf;
    }
    public override Item Copy()
    {
        QuirkShard_Item tmp = base.Copy() as QuirkShard_Item;
        tmp.quirkShardOf = quirkShardOf;
        return tmp;
    }
}
[System.Serializable]
public class QuirkShardSave
{
    public QuirkData quirkShardOf;
}
