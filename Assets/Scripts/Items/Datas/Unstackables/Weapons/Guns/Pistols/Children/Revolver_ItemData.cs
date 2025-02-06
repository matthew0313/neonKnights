using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "Revolver", menuName = "Scriptables/Items/Unstackables/Weapons/Guns/Pistols/Revolver", order = 0)]
public class Revolver_ItemData : PistolItemData
{
    [Header("Revolver")]
    [SerializeField] float m_scopeSpreadReduction;
    [SerializeField] int m_scopeCost;
    public float scopeSpreadReduction => m_scopeSpreadReduction;
    public int scopeCost => m_scopeCost;
    public override Item Create()
    {
        return new Revolver_Item(this);
    }
}
public class Revolver_Item : PistolItem, IScopeAttachable
{
    new public readonly Revolver_ItemData data;
    new public Revolver weapon => base.weapon as Revolver;
    public Revolver_Item(Revolver_ItemData data) : base(data)
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
        Revolver_Model tmp = base.GetModel() as Revolver_Model;
        tmp.scope.gameObject.SetActive(scopeAttached);
        return tmp;
    }
    public override Item Copy()
    {
        Revolver_Item tmp = base.Copy() as Revolver_Item;
        tmp.scopeAttached = scopeAttached;
        return tmp;
    }
}