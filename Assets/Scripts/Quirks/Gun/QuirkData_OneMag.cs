using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Quirk_OneMag", menuName = "Scriptables/Quirks/Guns/OneMag", order = 0)]
public class QuirkData_OneMag : QuirkData
{
    [Header("OneMag")]
    [SerializeField] float m_minConvertPercentage;
    [SerializeField] float m_maxConvertPercentage;
    public float minConvertPercentage => m_minConvertPercentage;
    public float maxConvertPercentage => m_maxConvertPercentage;
    public override bool IsAddable(IQuirkable target)
    {
        return base.IsAddable(target) && target is GunItem;
    }
    public override Quirk Create()
    {
        return new Quirk_OneMag(this);
    }
}
public class Quirk_OneMag : Quirk
{
    new public readonly QuirkData_OneMag data;
    public Quirk_OneMag(QuirkData_OneMag data) : base(data)
    {
        this.data = data;
    }
    float convertPercentage = 50.0f;
    public override void OnQuirkAdd(IQuirkable target)
    {
        base.OnQuirkAdd(target);
        int bonus = Mathf.RoundToInt((target as GunItem).reserve * convertPercentage / 100.0f);
        (target as GunItem).magSize += bonus;
        (target as GunItem).reserve = 0;
        (target as GunItem).mag += bonus;
        (target as GunItem).reserveLeft = 0;
        (target as GunItem).onAmmoRefill += OnAmmoRefill;
    }
    void OnAmmoRefill(GunAmmoRefillInfo info)
    {
        (target as GunItem).mag = Mathf.Min((target as GunItem).magSize, (target as GunItem).mag + Mathf.CeilToInt((target as GunItem).magSize * info.refillPercentage / 100.0f));
    }
    public override void SetRandom()
    {
        base.SetRandom();
        convertPercentage = Mathf.Round(UnityEngine.Random.Range(data.minConvertPercentage, data.maxConvertPercentage));
    }
    public override LangText Describe()
    {
        return new LangText()
        {
            en = $"{convertPercentage}% reserve => magSize, no reserve",
            kr = $"{convertPercentage}% 탄환 저장량 => 탄창 크기"
        };
    }
    public override void Save(DataUnit data)
    {
        base.Save(data);
        data.floats["convertPercentage"] = convertPercentage;
    }
    public override void Load(DataUnit data)
    {
        base.Load(data);
        data.floats.TryGetValue("convertPercentage", out convertPercentage);
    }
    public override Quirk Copy()
    {
        Quirk_OneMag tmp = base.Copy() as Quirk_OneMag;
        tmp.convertPercentage = convertPercentage;
        return tmp;
    }
}