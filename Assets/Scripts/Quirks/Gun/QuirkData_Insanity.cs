using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Quirk_Insanity", menuName = "Scriptables/Quirks/Guns/Insanity", order = 0)]
public class QuirkData_Insanity : QuirkData
{
    [Header("Insanity")]
    [SerializeField] float m_fireRateBonusPercentage;
    [SerializeField] float m_damageReductionPercentage;
    public float fireRateBonusPercentage => m_fireRateBonusPercentage;
    public float damageReductionPercentage => m_damageReductionPercentage;
    public override bool IsAddable(IQuirkable target)
    {
        return base.IsAddable(target) && target is GunItem;
    }
    public override Quirk Create()
    {
        return new Quirk_Insanity(this);
    }
}
public class Quirk_Insanity : Quirk
{
    new public readonly QuirkData_Insanity data;
    public Quirk_Insanity(QuirkData_Insanity data) : base(data)
    {
        this.data = data;
    }
    public override void OnQuirkAdd(IQuirkable target)
    {
        base.OnQuirkAdd(target);
        (target as GunItem).fireRate *= 1.0f - data.fireRateBonusPercentage / 100.0f;
        (target as GunItem).damage *= 1.0f - data.damageReductionPercentage / 100.0f;
    }
    public override LangText Describe()
    {
        return new LangText()
        {
            en = $"-{data.damageReductionPercentage} Damage +{data.fireRateBonusPercentage}% FireRate",
            kr = $"-{data.damageReductionPercentage} 대미지 +{data.fireRateBonusPercentage}% 발사주기"
        };
    }
}