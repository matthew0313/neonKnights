using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Quirk_Vampire", menuName = "Scriptables/Quirks/Melees/Vampire", order = 0)]
public class QuirkData_Vampire : QuirkData
{
    [Header("Vampire")]
    [SerializeField] float m_healthRestored;
    public float healthRestored => m_healthRestored;
    public override bool IsAddable(IQuirkable target)
    {
        return base.IsAddable(target) && target is MeleeItem;
    }
    public override Quirk Create()
    {
        return new Quirk_Vampire(this);
    }
}
public class Quirk_Vampire : Quirk
{
    new public readonly QuirkData_Vampire data;
    public Quirk_Vampire(QuirkData_Vampire data) : base(data)
    {
        this.data = data;
    }
    public override void OnQuirkAdd(IQuirkable target)
    {
        base.OnQuirkAdd(target);
        (target as MeleeItem).onSlashDamageGive += OnSlashDamageGive;
    }
    void OnSlashDamageGive(MeleeSlash slash, DamageReceivedInfo info)
    {
        (target as MeleeItem).wielder.hpComp.GetHeal(data.healthRestored);
    }
    public override LangText Describe()
    {
        return new LangText()
        {
            en = $"+{data.healthRestored} HP Every hit",
            kr = $"명중 시 +{data.healthRestored} HP"
        };
    }
}