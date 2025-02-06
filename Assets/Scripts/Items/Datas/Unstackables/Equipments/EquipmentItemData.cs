using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class EquipmentItemData : UnstackableItemData
{
    
}
public abstract class EquipmentItem : UnstackableItem, IStatDisplayed, IQuirkable, IQuirkDisplayed
{
    new public readonly EquipmentItemData data;
    protected EquipmentItem(EquipmentItemData data) : base(data)
    {
        this.data = data;
    }
    public List<Quirk> quirks { get; } = new();
    public Player equipper { get; private set; } = null;

    public Action<Player> onEquip, onUnequip;
    public Action onEquipUpdate;
    public virtual void OnEquip(Player equipper)
    {
        this.equipper = equipper;
        onEquip?.Invoke(equipper);
    }
    public virtual void OnEquipUpdate()
    {
        onEquipUpdate?.Invoke();
    }
    public virtual void OnUnequip(Player equipper)
    {
        this.equipper = null;
        onUnequip?.Invoke(equipper); 
    }
    public abstract IEnumerable<LangText> GetStats();

    public IEnumerable<Quirk> GetQuirks()
    {
        foreach (var i in quirks) yield return i;
    }
    public override Item Copy()
    {
        EquipmentItem tmp = base.Copy() as EquipmentItem;
        foreach(var i in quirks)
        {
            (i as IQuirkable).AddQuirk(i.Copy());
        }
        return tmp;
    }
}