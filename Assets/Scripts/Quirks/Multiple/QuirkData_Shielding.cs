using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Quirk_Shielding", menuName = "Scriptables/Quirks/Multiple/Shielding", order = 0)]
public class QuirkData_Shielding : QuirkData
{
    [Header("Shielding")]
    [SerializeField] float m_minShieldBonus;
    [SerializeField] float m_maxShieldBonus;
    public float minShieldBonus => m_minShieldBonus;
    public float maxShieldBonus => m_maxShieldBonus;
    public override bool IsAddable(IQuirkable target)
    {
        return base.IsAddable(target);
    }
    public override Quirk Create()
    {
        return new Quirk_Shielding(this);
    }
}
public class Quirk_Shielding : Quirk
{
    new public readonly QuirkData_Shielding data;
    public Quirk_Shielding(QuirkData_Shielding data) : base(data)
    {
        this.data = data;
    }
    float shieldBonus = 0.0f;
    bool added = false;
    public override void OnQuirkAdd(IQuirkable target)
    {
        base.OnQuirkAdd(target);
        (target as Item).onWield += (Player wielder) =>
        {
            if (!added)
            {
                wielder.hpComp.maxShield += shieldBonus;
                added = true;
            }
        };
        (target as Item).onUnwield += (Player wielder) =>
        {
            if (added)
            {
                wielder.hpComp.maxShield -= shieldBonus;
                added = false;
            }
        };
    }
    public override void SetRandom()
    {
        shieldBonus = Mathf.Round(UnityEngine.Random.Range(data.minShieldBonus, data.maxShieldBonus));
    }
    public override LangText Describe()
    {
        return new LangText()
        {
            en = $"+{shieldBonus} shield when held",
            kr = $"들고 있으면 +{shieldBonus} 실드"
        };
    }
    public override void Save(DataUnit data)
    {
        base.Save(data);
        data.floats["shieldBonus"] = shieldBonus;
    }
    public override void Load(DataUnit data)
    {
        base.Load(data);
        data.floats.TryGetValue("shieldBonus", out shieldBonus);
    }
    public override Quirk Copy()
    {
        Quirk_Shielding tmp = base.Copy() as Quirk_Shielding;
        tmp.shieldBonus = shieldBonus;
        return tmp;
    }
}