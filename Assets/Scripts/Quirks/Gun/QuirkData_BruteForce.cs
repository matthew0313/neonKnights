using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Quirk_BruteForce", menuName = "Scriptables/Quirks/Guns/BruteForce", order = 0)]
public class QuirkData_BruteForce : QuirkData
{
    [Header("BruteForce")]
    [SerializeField] float m_minDamageBonusPercentage;
    [SerializeField] float m_maxDamageBonusPercentage;
    public float minDamageBonusPercentage => m_minDamageBonusPercentage;
    public float maxDamageBonusPercentage => m_maxDamageBonusPercentage;
    public override bool IsAddable(IQuirkable target)
    {
        return base.IsAddable(target) && target is GunItem;
    }
    public override Quirk Create()
    {
        return new Quirk_BruteForce(this);
    }
}
public class Quirk_BruteForce : Quirk
{
    new public readonly QuirkData_BruteForce data;
    public Quirk_BruteForce(QuirkData_BruteForce data) : base(data)
    {
        this.data = data;
    }
    float damageBonusPercentage = 0.0f;
    public override void OnQuirkAdd(IQuirkable target)
    {
        base.OnQuirkAdd(target);
        (target as GunItem).modifyShotDamageGiven += ModifyShotDamageGiven;
    }
    void ModifyShotDamageGiven(GunShot shot, GunGiveDamageInfo info)
    {
        if (info.hit.hitObject.TryGetComponent(out ResistPoint tmp)) info.damage.amount *= (100.0f + damageBonusPercentage) / 100.0f;
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
            kr = $"+{damageBonusPercentage}% 방어 포인트 대미지"
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
        Quirk_BruteForce tmp = base.Copy() as Quirk_BruteForce;
        tmp.damageBonusPercentage = damageBonusPercentage;
        return tmp;
    }
}