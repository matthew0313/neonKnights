using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.Playables;

[CreateAssetMenu(fileName = "Quirk_EagleEyed", menuName = "Scriptables/Quirks/Guns/EagleEyed", order = 0)]
public class QuirkData_EagleEyed : QuirkData
{
    [Header("EagleEyed")]
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
        return new Quirk_EagleEyed(this);
    }
}
public class Quirk_EagleEyed : Quirk
{
    new public readonly QuirkData_EagleEyed data;
    public Quirk_EagleEyed(QuirkData_EagleEyed data) : base(data)
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
        if (info.hit.hitObject.TryGetComponent(out WeakPoint tmp)) info.damage.amount *= (100.0f + damageBonusPercentage) / 100.0f;
    }
    public override void SetRandom()
    {
        damageBonusPercentage = Mathf.Round(UnityEngine.Random.Range(data.minDamageBonusPercentage, data.maxDamageBonusPercentage));
    }
    public override LangText quirkName => new LangText()
    {
        en = "Eagle-Eyed",
        kr = "매의 눈"
    };
    public override LangText quirkDescription => new LangText()
    {
        en = "Increases damage against weak points.",
        kr = "약점에 주는 대미지가 증가합니다."
    };
    public override LangText Describe()
    {
        return new LangText()
        {
            en = $"+{damageBonusPercentage}% damage to weak",
            kr = $"+{damageBonusPercentage}% 약점 대미지"
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
        Quirk_EagleEyed tmp = base.Copy() as Quirk_EagleEyed;
        tmp.damageBonusPercentage = damageBonusPercentage;
        return tmp;
    }
}