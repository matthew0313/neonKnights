using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Quirk_Piercing", menuName = "Scriptables/Quirks/Guns/Piercing", order = 0)]
public class QuirkData_Piercing : QuirkData
{
    [Header("Piercing")]
    [SerializeField] float m_minPierceChance;
    [SerializeField] float m_maxPierceChance;
    public float minPierceChance => m_minPierceChance;
    public float maxPierceChance => m_maxPierceChance;
    public override bool IsAddable(IQuirkable target)
    {
        return base.IsAddable(target) && target is GunItem;
    }
    public override Quirk Create()
    {
        return new Quirk_Piercing(this);
    }
}
public class Quirk_Piercing : Quirk
{
    new public readonly QuirkData_Piercing data;
    public Quirk_Piercing(QuirkData_Piercing data) : base(data)
    {
        this.data = data;
    }
    float pierceChance = 0.0f;
    public override void OnQuirkAdd(IQuirkable target)
    {
        base.OnQuirkAdd(target);
        (target as GunItem).modifyFireInfo += (GunFireInfo info) =>
        {
            if (UnityEngine.Random.Range(0.0f, 100.0f) <= pierceChance) info.pierce++;
        };
    }
    public override void SetRandom()
    {
        pierceChance = Mathf.Round(UnityEngine.Random.Range(data.minPierceChance, data.maxPierceChance));
    }
    public override LangText quirkName => new LangText()
    {
        en = "Piercing",
        kr = "관통"
    };
    public override LangText quirkDescription => new LangText()
    {
        en = "Bullets may get 1 pierce",
        kr = "탄환이 확률적으로 1회 관통력을 가집니다."
    };
    public override LangText Describe()
    {
        return new LangText()
        {
            en = $"{pierceChance}% +1 pierce",
            kr = $"{pierceChance}% +1 관통"
        };
    }
    public override void Save(DataUnit data)
    {
        base.Save(data);
        data.floats["pierceChance"] = pierceChance;
    }
    public override void Load(DataUnit data)
    {
        base.Load(data);
        data.floats.TryGetValue("pierceChance", out pierceChance);
    }
    public override Quirk Copy()
    {
        Quirk_Piercing tmp = base.Copy() as Quirk_Piercing;
        tmp.pierceChance = pierceChance;
        return tmp;
    }
}