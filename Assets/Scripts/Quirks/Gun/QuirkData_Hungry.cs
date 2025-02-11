using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Quirk_Hungry", menuName = "Scriptables/Quirks/Guns/Hungry", order = 0)]
public class QuirkData_Hungry : QuirkData
{
    [Header("Hungry")]
    [SerializeField] float m_minDamageBonus;
    [SerializeField] float m_maxDamageBonus;
    [SerializeField] float m_bulletConsumeChance;
    [SerializeField] int m_maxBulletConsumed;
    public float minDamageBonus => m_minDamageBonus;
    public float maxDamageBonus => m_maxDamageBonus;
    public float bulletConsumeChance => m_bulletConsumeChance;
    public int maxBulletConsumed => m_maxBulletConsumed;
    public override bool IsAddable(IQuirkable target)
    {
        return base.IsAddable(target) && target is GunItem;
    }
    public override Quirk Create()
    {
        return new Quirk_Hungry(this);
    }
}
public class Quirk_Hungry : Quirk
{
    new public readonly QuirkData_Hungry data;
    public Quirk_Hungry(QuirkData_Hungry data) : base(data)
    {
        this.data = data;
    }
    public override void OnQuirkAdd(IQuirkable target)
    {
        base.OnQuirkAdd(target);
        (target as GunItem).modifyFireInfo += ModifyFireInfo;
    }
    float damageBonus = 100.0f;
    public override void SetRandom()
    {
        base.SetRandom();
        damageBonus = Mathf.Round(UnityEngine.Random.Range(data.minDamageBonus, data.maxDamageBonus));
    }
    void ModifyFireInfo(GunFireInfo info)
    {
        if (info.source != GunFireSource.GunFire) return;
        int consume = 0;
        while (consume < data.maxBulletConsumed && UnityEngine.Random.Range(0.0f, 100.0f) < data.bulletConsumeChance) consume++;
        info.consumedBullet += consume;
        info.damage *= 1.0f + (damageBonus * consume) / 100.0f;
    }
    public override LangText Describe()
    {
        return new LangText()
        {
            en = $"+{damageBonus}% damage per bullet consumed",
            kr = $"소비한 탄환 개수만큼 +{damageBonus}% 대미지"
        };
    }
}