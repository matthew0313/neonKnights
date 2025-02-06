using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Quirk_Slicing", menuName = "Scriptables/Quirks/Melees/Slicing", order = 0)]
public class QuirkData_Slicing : QuirkData
{
    [Header("Slicing")]
    [SerializeField] int m_maxHitsBonus;
    public int maxHitsBonus => m_maxHitsBonus;
    public override bool IsAddable(IQuirkable target)
    {
        return base.IsAddable(target) && target is MeleeItem;
    }
    public override Quirk Create()
    {
        return new Quirk_Slicing(this);
    }
}
public class Quirk_Slicing : Quirk
{
    new public readonly QuirkData_Slicing data;
    public Quirk_Slicing(QuirkData_Slicing data) : base(data)
    {
        this.data = data;
    }
    public override void OnQuirkAdd(IQuirkable target)
    {
        base.OnQuirkAdd(target);
        (target as MeleeItem).maxHits += data.maxHitsBonus;
    }
    public override LangText Describe()
    {
        return new LangText()
        {
            en = $"+{data.maxHitsBonus} Max Hits",
            kr = $"+{data.maxHitsBonus} 광역 히트 수"
        };
    }
}