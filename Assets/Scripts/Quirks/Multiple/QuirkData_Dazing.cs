using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Quirk_Dazing", menuName = "Scriptables/Quirks/Guns/Dazing", order = 0)]
public class QuirkData_Dazing : QuirkData
{
    [Header("Dazing")]
    [SerializeField] float m_minStunChance;
    [SerializeField] float m_maxStunChance;
    public float minStunChance => m_minStunChance;
    public float maxStunChance => m_maxStunChance;
    public override bool IsAddable(IQuirkable target)
    {
        return base.IsAddable(target) && (target is GunItem || target is MeleeItem);
    }
    public override Quirk Create()
    {
        return new Quirk_Dazing(this);
    }
}
public class Quirk_Dazing : Quirk
{
    new public readonly QuirkData_Dazing data;
    public Quirk_Dazing(QuirkData_Dazing data) : base(data)
    {
        this.data = data;
    }
    float stunChance;
    public override void OnQuirkAdd(IQuirkable target)
    {
        base.OnQuirkAdd(target);
        if (target is GunItem tmp) tmp.onShotHit += OnShotHit;
        else if (target is MeleeItem tmp2) tmp2.onSlashHit += OnSlashHit;
    }
    void OnShotHit(GunShot shot, GunHitInfo info)
    {
        if (info.hitObject.TryGetComponent<WeakPoint>(out _))
        {
            if (info.hitObject.TryGetComponent(out IAlimentable tmp))
            {
                if (UnityEngine.Random.Range(0.0f, 100.0f) <= stunChance) tmp.ApplyAliment(new Aliment_Stunned(1.0f));
            }
        }
    }
    void OnSlashHit(MeleeSlash slash, GameObject hitObject)
    {
        if (hitObject.TryGetComponent<WeakPoint>(out _))
        {
            if (hitObject.TryGetComponent(out IAlimentable tmp))
            {
                if (UnityEngine.Random.Range(0.0f, 100.0f) <= stunChance) tmp.ApplyAliment(new Aliment_Stunned(1.0f));
            }
        }
    }
    public override void SetRandom()
    {
        base.SetRandom();
        stunChance = Mathf.Round(UnityEngine.Random.Range(data.minStunChance, data.maxStunChance));
    }
    public override LangText Describe()
    {
        return new LangText()
        {
            en = $"{stunChance}% stun on weakpoints",
            kr = $"약점 히트 시 {stunChance}% 스턴"
        };
    }
    public override Quirk Copy()
    {
        Quirk_Dazing tmp = base.Copy() as Quirk_Dazing;
        tmp.stunChance = stunChance;
        return tmp;
    }
}