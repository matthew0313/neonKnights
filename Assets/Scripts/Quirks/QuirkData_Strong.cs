using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Quirk_Strong", menuName = "Scriptables/Quirks/Guns/Strong", order = 0)]
public class QuirkData_Strong : QuirkData
{
    [Header("Strong")]
    [SerializeField] float m_minDamageBonus;
    [SerializeField] float m_maxDamageBonus;
    public float minDamageBonus => m_minDamageBonus;
    public float maxDamageBonus => m_maxDamageBonus;
    public override bool IsAddable(WeaponItem target)
    {
        return base.IsAddable(target) && target is GunItem;
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
    float damageBonus = 0.0f;
    public override void OnQuirkAdd(WeaponItem target)
    {
        base.OnQuirkAdd(target);
        (target as GunItem).damage *= (1.0f + damageBonus);
    }
    public override void SetRandom()
    {
        damageBonus = Mathf.Round(UnityEngine.Random.Range(data.minDamageBonus, data.maxDamageBonus) * 100.0f) / 100.0f;
    }
    public override LangText Describe()
    {
        return new LangText()
        {
            en = $"+{damageBonus * 100.0f}% damage",
            kr = $"+{damageBonus * 100.0f}% 대미지"
        };
    }
    public override void Save(DataUnit data)
    {
        base.Save(data);
        data.floats["damageBonus"] = damageBonus;
    }
    public override void Load(DataUnit data)
    {
        base.Load(data);
        data.floats.TryGetValue("damageBonus", out damageBonus);
    }
}