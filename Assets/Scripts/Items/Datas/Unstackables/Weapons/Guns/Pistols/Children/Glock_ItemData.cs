using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "Glock", menuName = "Scriptables/Items/Unstackables/Weapons/Guns/Pistols/Glock", order = 0)]
public class Glock_ItemData : PistolItemData
{
    [Header("Glock")]
    [SerializeField] float m_scopeSpreadReduction;
    [SerializeField] int m_scopeCost;
    public float scopeSpreadReduction => m_scopeSpreadReduction;
    public int scopeCost => m_scopeCost;
    public override Item Create()
    {
        return new Glock_Item(this);
    }
}
public class Glock_Item : PistolItem, IScopeAttachable
{
    new public readonly Glock_ItemData data;
    new public Glock weapon => base.weapon as Glock;
    public Glock_Item(Glock_ItemData data) : base(data)
    {
        this.data = data;
    }
    bool m_scopeAttached = false;
    public bool scopeAttached
    {
        get => m_scopeAttached;
        set
        {
            m_scopeAttached = value;
            weapon.scope.SetActive(value);
            UpdateModel();
        }
    }
    public float scopeSpreadReduction => data.scopeSpreadReduction;
    public int scopeCost => data.scopeCost;
    public override ItemModel GetModel()
    {
        Debug.Log("RAWADCWw");
        Glock_Model tmp = base.GetModel() as Glock_Model;
        tmp.scope.gameObject.SetActive(scopeAttached);
        return tmp;
    }
}