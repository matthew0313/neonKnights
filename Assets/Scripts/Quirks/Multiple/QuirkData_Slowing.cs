using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Quirk_Slowing", menuName = "Scriptables/Quirks/Guns/Slowing", order = 0)]
public class QuirkData_Slowing : QuirkData
{
    [Header("Strong")]
    [SerializeField] float m_minSlowChance;
    [SerializeField] float m_maxSlowChance;
    [SerializeField] float m_minSlowPercentage, m_maxSlowPercentage;
    public float minSlowChance => m_minSlowChance;
    public float maxSlowChance => m_maxSlowChance;
    public float minSlowPercentage => m_minSlowPercentage;
    public float maxSlowPercentage => m_maxSlowPercentage;
    public override bool IsAddable(IQuirkable target)
    {
        return base.IsAddable(target) && (target is GunItem || target is MeleeItem);
    }
    public override Quirk Create()
    {
        return new Quirk_Slowing(this);
    }
}
public class Quirk_Slowing : Quirk
{
    new public readonly QuirkData_Slowing data;
    public Quirk_Slowing(QuirkData_Slowing data) : base(data)
    {
        this.data = data;
    }
    float slowChance;
    public override void OnQuirkAdd(IQuirkable target)
    {
        base.OnQuirkAdd(target);
        if (target is GunItem tmp) tmp.onShotHit += OnShotHit;
        else if (target is MeleeItem tmp2) tmp2.onSlashHit += OnSlashHit;
    }
    public override void SetRandom()
    {
        slowChance = Mathf.Round(UnityEngine.Random.Range(data.minSlowChance, data.maxSlowChance));
    }
    void OnShotHit(GunShot shot, GunHitInfo info)
    {
        if(info.hitObject.TryGetComponent(out IAlimentable tmp))
        {
            if (UnityEngine.Random.Range(0.0f, 100.0f) <= slowChance)
            {
                tmp.ApplyAliment(new Aliment_Slowed(UnityEngine.Random.Range(data.minSlowPercentage, data.maxSlowPercentage), 3.0f));
            }
        }
    }
    void OnSlashHit(MeleeSlash slash, GameObject hitObject)
    {
        if (hitObject.TryGetComponent(out IAlimentable tmp))
        {
            if (UnityEngine.Random.Range(0.0f, 100.0f) <= slowChance)
            {
                tmp.ApplyAliment(new Aliment_Slowed(UnityEngine.Random.Range(data.minSlowPercentage, data.maxSlowPercentage), 3.0f));
            }
        }
    }
    public override LangText Describe()
    {
        return new LangText()
        {
            en = $"{slowChance}% 3 sec speed debuff({data.minSlowPercentage}%~{data.maxSlowPercentage})",
            kr = $"{slowChance}% 3초 감속 디버프({data.minSlowPercentage}%~{data.maxSlowPercentage})"
        };
    }
    public override void Save(DataUnit data)
    {
        base.Save(data);
        data.floats["slowChance"] = slowChance;
    }
    public override void Load(DataUnit data)
    {
        base.Load(data);
        data.floats.TryGetValue("slowChance", out slowChance);
    }
    public override Quirk Copy()
    {
        Quirk_Slowing tmp = base.Copy() as Quirk_Slowing;
        tmp.slowChance = slowChance;
        return tmp;
    }
}