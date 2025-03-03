using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Quirk_Striking", menuName = "Scriptables/Quirks/Melees/Striking", order = 0)]
public class QuirkData_Striking : QuirkData
{
    [Header("Striking")]
    [SerializeField] float m_minStunChance;
    [SerializeField] float m_maxStunChance;
    public float minStunChance => m_minStunChance;
    public float maxStunChance => m_maxStunChance;
    public override bool IsAddable(IQuirkable target)
    {
        return base.IsAddable(target) && target is MeleeItem;
    }
    public override Quirk Create()
    {
        return new Quirk_Striking(this);
    }
}
public class Quirk_Striking : Quirk
{
    new public readonly QuirkData_Striking data;
    public Quirk_Striking(QuirkData_Striking data) : base(data)
    {
        this.data = data;
    }
    float stunChance;
    public override void OnQuirkAdd(IQuirkable target)
    {
        base.OnQuirkAdd(target);
        (target as MeleeItem).onSlashHit += OnSlashHit;
    }
    void OnSlashHit(MeleeSlash slash, GameObject hit)
    {
        if(hit.TryGetComponent(out IAlimentable tmp))
        {
            if (UnityEngine.Random.Range(0.0f, 100.0f) <= stunChance) tmp.ApplyAliment(new Aliment_Stunned(1.0f));
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
            en = $"{stunChance}% stun on hit",
            kr = $"히트 시 {stunChance}% 스턴"
        };
    }
    public override Quirk Copy()
    {
        Quirk_Striking tmp = base.Copy() as Quirk_Striking;
        tmp.stunChance = stunChance;
        return tmp;
    }
}