using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Quirk_Strong", menuName = "Scriptables/Quirks/Multiple/Strong", order = 0)]
public class QuirkData_Strong : QuirkData
{
    [Header("Strong")]
    [SerializeField] float m_minDamageBonusPercentage;
    [SerializeField] float m_maxDamageBonusPercentage;
    public float minDamageBonusPercentage => m_minDamageBonusPercentage;
    public float maxDamageBonusPercentage => m_maxDamageBonusPercentage;
    public override bool IsAddable(IQuirkable target)
    {
        return base.IsAddable(target) && target is IDamageModifyable;
    }
    public override Quirk Create()
    {
        return new Quirk_Strong(this);
    }
}
public class Quirk_Strong : Quirk
{
    new public readonly QuirkData_Strong data;
    public Quirk_Strong(QuirkData_Strong data) : base(data)
    {
        this.data = data;
    }
    float damageBonusPercentage = 0.0f;
    public override void OnQuirkAdd(IQuirkable target)
    {
        base.OnQuirkAdd(target);
        (target as IDamageModifyable).damage *= (100.0f + damageBonusPercentage) / 100.0f;
    }
    public override void SetRandom()
    {
        damageBonusPercentage = Mathf.Round(UnityEngine.Random.Range(data.minDamageBonusPercentage, data.maxDamageBonusPercentage));
    }
    public override LangText Describe()
    {
        return new LangText()
        {
            en = $"+{damageBonusPercentage}% damage",
            kr = $"+{damageBonusPercentage}% 대미지"
        };
    }
    public override void Save(DataUnit data)
    {
        base.Save(data);
        data.floats["damageBonusPercentage"] = damageBonusPercentage;
    }
    public override void Load(DataUnit data)
    {
        base.Load(data);
        data.floats.TryGetValue("damageBonusPercentage", out damageBonusPercentage);
    }
    public override Quirk Copy()
    {
        Quirk_Strong tmp = base.Copy() as Quirk_Strong;
        tmp.damageBonusPercentage = damageBonusPercentage;
        return tmp;
    }
}