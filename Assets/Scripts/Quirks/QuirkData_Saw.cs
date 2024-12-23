using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Quirk_Saw", menuName = "Scriptables/Quirks/Guns/Saw", order = 0)]
public class QuirkData_Saw : QuirkData
{
    [Header("Saw")]
    [SerializeField] float m_maxRange;
    public override bool IsAddable(WeaponItem target)
    {
        return base.IsAddable(target) && target is GunItem;
    }
    public override Quirk Create()
    {
        return new Quirk_Saw(this);
    }
}
public class Quirk_Saw : Quirk
{
    new public readonly QuirkData_Saw data;
    public Quirk_Saw(QuirkData_Saw data) : base(data)
    {
        this.data = data;
    }
    public override void OnQuirkAdd(WeaponItem target)
    {
        base.OnQuirkAdd(target);
    }
    public override LangText Describe()
    {
        throw new NotImplementedException();
    }
}