using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Quirk_Breaker", menuName = "Scriptables/Quirks/Multiple/Breaker", order = 0)]
public class QuirkData_Breaker : QuirkData
{
    [Header("Breaker")]
    [SerializeField] float m_minDamageBonusPercentage;
    [SerializeField] float m_maxDamageBonusPercentage;
    public float minDamageBonusPercentage => m_minDamageBonusPercentage;
    public float maxDamageBonusPercentage => m_maxDamageBonusPercentage;
    public override bool IsAddable(IQuirkable target)
    {
        return base.IsAddable(target) && (target is GunItem || target is MeleeItem);
    }
    public override Quirk Create()
    {
        return new Quirk_Breaker(this);
    }
}
public class Quirk_Breaker : Quirk
{
    new public readonly QuirkData_Breaker data;
    public Quirk_Breaker(QuirkData_Breaker data) : base(data)
    {
        this.data = data;
    }
    float damageBonusPercentage = 0.0f;
    public override void OnQuirkAdd(IQuirkable target)
    {
        base.OnQuirkAdd(target);
        if (target is GunItem tmp) tmp.modifyShotDamageGiven += ModifyShotDamageGiven;
        else if (target is MeleeItem tmp2) tmp2.modifySlashDamageGiven += ModifySlashDamageGiven;
    }
    void ModifyShotDamageGiven(GunShot shot, GunGiveDamageInfo info)
    {
        if (info.hit.hitObject.TryGetComponent(out ResistPoint tmp)) info.damage.amount *= (100.0f + damageBonusPercentage) / 100.0f;
    }
    void ModifySlashDamageGiven(MeleeSlash slash, MeleeGiveDamageInfo info)
    {
        if (info.hitObject.TryGetComponent(out ResistPoint tmp)) info.damage.amount *= (100.0f + damageBonusPercentage) / 100.0f;
    }
    public override void SetRandom()
    {
        damageBonusPercentage = Mathf.Round(UnityEngine.Random.Range(data.minDamageBonusPercentage, data.maxDamageBonusPercentage));
    }
    public override LangText Describe()
    {
        return new LangText()
        {
            en = $"+{damageBonusPercentage}% damage to resist",
            kr = $"+{damageBonusPercentage}% 내성 포인트 대미지"
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
        Quirk_Breaker tmp = base.Copy() as Quirk_Breaker;
        tmp.damageBonusPercentage = damageBonusPercentage;
        return tmp;
    }
}